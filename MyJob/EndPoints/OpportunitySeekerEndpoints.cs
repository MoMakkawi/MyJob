using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using MyJob.Database;
using MyJob.Models;
using MyJob.DTOs.OpportunitySeekerDTOs;

namespace MyJob.EndPoints;

public static class OpportunitySeekerEndpoints
{
    public static void MapOpportunitySeekerEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/OpportunitySeeker").WithTags(nameof(OpportunitySeeker));

        SearchOpportunitySeekersEndPoint(group);
        UpdateOpportunitySeekerEndPoint(group);
        CreateOpportunitySeekerEndPoint(group);
        DeleteOpportunitySeekerEndPoint(group);
    }
    private static void DeleteOpportunitySeekerEndPoint(RouteGroupBuilder group)
    {
        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, MyJobContext db) =>
        {
            var affected = await db.OpportunitySeekers
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteOpportunitySeeker")
        .WithOpenApi();
    }

    private static void CreateOpportunitySeekerEndPoint(RouteGroupBuilder group)
    {
        group.MapPost("/", async (OpportunitySeekerCommandDTO commandOpportunitySeeker, MyJobContext db) =>
        {

            var experiences =
            (from opportunity in db.Opportunities.AsEnumerable()
             from experienceId in commandOpportunitySeeker.ExperiencesIds ?? []
             where opportunity.Id == experienceId
             select opportunity).ToList();

            OpportunitySeeker opportunitySeeker = new()
            {
                FirstName = commandOpportunitySeeker.FirstName,
                LastName = commandOpportunitySeeker.LastName,
                About = commandOpportunitySeeker.About,
                Email = commandOpportunitySeeker.Email,
                Password = commandOpportunitySeeker.Password,
                PhoneNumber = commandOpportunitySeeker.PhoneNumber,
                Specialty = commandOpportunitySeeker.Specialty,
                Experiences = experiences,
                CVId = commandOpportunitySeeker.CVId,
                PictureId = commandOpportunitySeeker.PictureId
            };

            db.OpportunitySeekers.Add(opportunitySeeker);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/OpportunitySeeker/{opportunitySeeker.Id}", opportunitySeeker);
        })
        .WithName("RegisterOpportunitySeeker")
        .WithOpenApi();
    }

    private static void UpdateOpportunitySeekerEndPoint(RouteGroupBuilder group)
    {
        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, OpportunitySeekerCommandDTO commandOpportunitySeeker, MyJobContext db) =>
        {
            var seeker = await db.OpportunitySeekers.FindAsync(id);
            if(seeker is null) return TypedResults.NotFound();

            seeker.FirstName = commandOpportunitySeeker.FirstName;
            seeker.LastName = commandOpportunitySeeker.LastName;
            seeker.Email = commandOpportunitySeeker.Email;
            seeker.Password = commandOpportunitySeeker.Password;
            seeker.PhoneNumber = commandOpportunitySeeker.PhoneNumber;
            seeker.Specialty = commandOpportunitySeeker.Specialty;
            seeker.About = commandOpportunitySeeker.About;

            seeker.Experiences = (from opportunity in db.Opportunities.AsEnumerable()
                                  from experienceId in commandOpportunitySeeker.ExperiencesIds ?? []
                                  where opportunity.Id == experienceId
                                  select opportunity).ToList();

            seeker.CVId = commandOpportunitySeeker.CVId;
            seeker.CV = await db.Files.FindAsync(commandOpportunitySeeker.CVId);

            seeker.PictureId = commandOpportunitySeeker.PictureId;
            seeker.Picture = await db.Files.FindAsync(commandOpportunitySeeker.PictureId);

            await db.SaveChangesAsync();

            return TypedResults.Ok();
        })
        .WithName("UpdateOpportunitySeeker")
        .WithOpenApi();
    }
    private static void SearchOpportunitySeekersEndPoint(RouteGroupBuilder group)
    {
        group.MapGet("/", (
            int? Id,
            string? FullName,
            string? Email,
            string? PhoneNumber,
            string? Specialty,
            int? PracticalExperienceMonthsNumber,
            int? VolunteerExperienceMonthsNumber,
            MyJobContext db ) =>
        {
            var seekers = db.OpportunitySeekers.AsEnumerable();

            if (Id is not null)
                seekers = seekers.Where(os => os.Id == Id);

            if (FullName is not null)
                seekers = seekers.Where(os => os.FullName == FullName);

            if (Email is not null)
                seekers = seekers.Where (os => os.Email == Email);

            if (PhoneNumber is not null)
                seekers = seekers.Where(os => os.PhoneNumber == PhoneNumber);

            if (Specialty is not null)
                seekers = seekers.Where(os => os.Specialty == Specialty);

            if (PracticalExperienceMonthsNumber is not null)
                seekers = seekers.Where(os => os.PracticalExperienceMonthsNumber == PracticalExperienceMonthsNumber);

            if (VolunteerExperienceMonthsNumber is not null)
                seekers = seekers.Where(os => os.VolunteerExperienceMonthsNumber == VolunteerExperienceMonthsNumber);


            return seekers.Select(os => os.ToDTO(db));
        })
        .WithName("OpportunitySeekersSearch")
        .WithOpenApi();
    }
}
