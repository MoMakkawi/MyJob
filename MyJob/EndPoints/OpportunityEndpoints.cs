﻿using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using MyJob.Database;
using MyJob.Models;
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
        group.MapPost("/", async (Opportunity opportunity, MyJobContext db) =>
        {
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
                    .SetProperty(m =>m.ApplicantsCVIds, opportunity.ApplicantsCVIds)
                    .SetProperty(m => m.OrganizationId, opportunity.OrganizationId)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateOpportunity")
        .WithOpenApi();
    }
    private static void GetOpportunityApplicantCVsEndPoint(RouteGroupBuilder group)
    {
        group.MapGet("/applicants-cv-paths/id={id}", async (int? id, MyJobContext db) =>
        {
            var opportunity = await db.Opportunities.FindAsync(id);

            return opportunity is null ? Results.NotFound() :
            Results.Ok(from applicantsCVId in opportunity.ApplicantsCVIds
                              from fileData in db.Files
                              where fileData.Id == applicantsCVId && File.Exists(fileData.Path)
                              select new { fileData.Path });
        })
        .WithName("GetOpportunityApplicantCVs")
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
