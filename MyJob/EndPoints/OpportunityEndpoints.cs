using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using MyJob.Database;
using MyJob.Models;
using MyJob.DTOs.OpportunityDTOs;
namespace MyJob.EndPoints;

public static class OpportunityEndpoints
{
    public static void MapOpportunityEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Opportunity").WithTags(nameof(Opportunity));

        UpdateOpportunityEndPoint(group);
        CreateOpportunityEndPoint(group);
        DeleteOpportunityEndPoint(group);
        SearchOpportunitiesEndPoint(group);
        GetOpportunityApplicantCVsEndPoint(group);
    }

    private static void DeleteOpportunityEndPoint(RouteGroupBuilder group)
    {
        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, MyJobContext db) =>
        {
            var affected = await db.Opportunities
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteOpportunity")
        .WithOpenApi();
    }

    private static void CreateOpportunityEndPoint(RouteGroupBuilder group)
    {
        group.MapPost("/", async Task<Results<Created<Opportunity>, BadRequest>> (OpportunityCommandDTO opportunityDTO, MyJobContext db) =>
        {
            if (opportunityDTO is { OrganizationId: null, OrganizationFullName: null })
                return TypedResults.BadRequest();

            var organizationFullName = opportunityDTO.OrganizationFullName;
            if (opportunityDTO.OrganizationId is not null)
            {
                var organization = await db.Organizations
                    .FindAsync(opportunityDTO.OrganizationId);

                if (organization is not null)
                    organizationFullName = organization.FullName;
            }

            var opportunity = new Opportunity()
            {
                StartDate = opportunityDTO.StartDate,
                EndDate = opportunityDTO.EndDate,
                OrganizationFullName = organizationFullName ?? string.Empty,
                OrganizationId = opportunityDTO.OrganizationId,
                Title = opportunityDTO.Title,
                Type = opportunityDTO.Type
            };

            db.Opportunities.Add(opportunity);

            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Opportunity/{opportunity.Id}", opportunity);
        })
        .WithName("CreateOpportunity")
        .WithOpenApi();
    }

    private static void UpdateOpportunityEndPoint(RouteGroupBuilder group)
    {
        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Opportunity opportunity, MyJobContext db) =>
        {
            var affected = await db.Opportunities
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Title, opportunity.Title)
                    .SetProperty(m => m.StartDate, opportunity.StartDate)
                    .SetProperty(m => m.EndDate, opportunity.EndDate)
                    .SetProperty(m => m.Type, opportunity.Type)
                    .SetProperty(m => m.OrganizationFullName, opportunity.OrganizationFullName)
                    .SetProperty(m =>m.ApplicantsIds, opportunity.ApplicantsIds)
                    .SetProperty(m => m.OrganizationId, opportunity.OrganizationId)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateOpportunity")
        .WithOpenApi();
    }
    private static void GetOpportunityApplicantCVsEndPoint(RouteGroupBuilder group)
    {
        group.MapGet("/applicants/id={id}", async (
            int id,
            string? Specialty,
            int? PracticalExperienceMonthsNumber,
            int? VolunteerExperienceMonthsNumber,
            MyJobContext db) =>
        {
            var opportunity = await db.Opportunities.FindAsync(id);

            if (opportunity is null) return Results.NotFound();

            var seekers = 
            from applicantId in opportunity.ApplicantsIds
            from seeker in db.OpportunitySeekers
            where seeker.Id == applicantId && seeker.CV is not null && File.Exists(seeker.CV.Path)
            select seeker;

            if (Specialty is not null)
                seekers = seekers.Where(os => os.Specialty.Contains(Specialty));

            if (PracticalExperienceMonthsNumber is not null)
                seekers = seekers.Where(os => os.PracticalExperienceMonthsNumber >= PracticalExperienceMonthsNumber);

            if (VolunteerExperienceMonthsNumber is not null)
                seekers = seekers.Where(os => os.VolunteerExperienceMonthsNumber >= VolunteerExperienceMonthsNumber);


            return Results.Ok(from seeker in seekers
                        select new
                        {
                            seeker.Id,
                            seeker.About,
                            seeker.FullName,
                            seeker.PhoneNumber,
                            seeker.Email,
                            seeker.PracticalExperienceMonthsNumber,
                            seeker.VolunteerExperienceMonthsNumber,
                            seeker.CV!.Path
                        } );
        })
        .WithName("GetOpportunityApplicant")
        .WithOpenApi();
    }
    private static void SearchOpportunitiesEndPoint(RouteGroupBuilder group)
    {
        group.MapGet("/", (
            int? Id,
            string? Title,
            DateOnly? StartDate,
            DateOnly? EndDate,
            OpportunityType? Type,
            int? MonthsNumber,
            string? OrganizationFullName,
            int? OrganizationId,
            MyJobContext db) =>
        {
            var opportunities = db.Opportunities.AsEnumerable();

            if (Id is not null)
                opportunities = opportunities.Where(o => o.Id == Id);

            if (OrganizationId is not null)
                opportunities = opportunities.Where(o => o.OrganizationId == OrganizationId);

            if (Title is not null)
                opportunities = opportunities.Where(o => o.Title == Title);

            if (Type is not null)
                opportunities = opportunities.Where(o => o.Type == Type);

            if (StartDate is not null)
                opportunities = opportunities.Where(o => o.StartDate == StartDate);

            if (EndDate is not null)
                opportunities = opportunities.Where(o => o.EndDate == EndDate);

            if (MonthsNumber is not null)
                opportunities = opportunities.Where(o => o.MonthsNumber == MonthsNumber);
            
            if (OrganizationFullName is not null)
                opportunities = opportunities.Where(o => o.OrganizationFullName.Contains(OrganizationFullName));

            return opportunities
            .Select(o => o.ToDTO(db));
        })
        .WithName("SearchOpportunities")
        .WithOpenApi();
    }
}
