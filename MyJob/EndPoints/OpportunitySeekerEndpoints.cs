using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using MyJob.Database;
using MyJob.Models;

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
        group.MapPost("/", async (OpportunitySeeker opportunitySeeker, MyJobContext db) =>
        {
            db.OpportunitySeekers.Add(opportunitySeeker);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/OpportunitySeeker/{opportunitySeeker.Id}", opportunitySeeker);
        })
        .WithName("RegisterOpportunitySeeker")
        .WithOpenApi();
    }

    private static void UpdateOpportunitySeekerEndPoint(RouteGroupBuilder group)
    {
        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, OpportunitySeeker opportunitySeeker, MyJobContext db) =>
        {
            var affected = await db.OpportunitySeekers
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.FirstName, opportunitySeeker.FirstName)
                    .SetProperty(m => m.LastName, opportunitySeeker.LastName)
                    .SetProperty(m => m.Email, opportunitySeeker.Email)
                    .SetProperty(m => m.Password, opportunitySeeker.Password)
                    .SetProperty(m => m.PhoneNumber, opportunitySeeker.PhoneNumber)
                    .SetProperty(m => m.Specialty, opportunitySeeker.Specialty)
                    .SetProperty(m => m.About, opportunitySeeker.About)
                    .SetProperty(m => m.CVId, opportunitySeeker.CVId)
                    .SetProperty(m => m.PictureId, opportunitySeeker.PictureId)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
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
