using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using MyJob.Database;
using MyJob.Models;
using System.Reflection;
using System.Text;
using MyJob.DTOs;
namespace MyJob.EndPoints;

public static class OpportunityEndpoints
{
    public static void MapOpportunityEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Opportunity").WithTags(nameof(Opportunity));

        GetAllOpportunitiesEndPoint(group);
        GetOpportunityByIdEndPoint(group);
        UpdateOpportunityEndPoint(group);
        CreateOpportunityEndPoint(group);
        DeleteOpportunityEndPoint(group);
        OpportunitiesSearchEndPoint(group);
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
            if (opportunity.OrganizationId is null) db.Opportunities.Add(opportunity);
            else db.Organizations
                .First(org => org.Id == opportunity.OrganizationId)
                .Opportunities.Add(opportunity);

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
                    .SetProperty(m => m.OrganizationId, opportunity.OrganizationId)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateOpportunity")
        .WithOpenApi();
    }

    private static void OpportunitiesSearchEndPoint(RouteGroupBuilder group)
    {
        // get the type of the class
        Type opportunityType = typeof(Opportunity);

        // get all the properties with the specified binding flags
        var opportunityProperties = opportunityType
            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Select(prop => new { prop.PropertyType, prop.Name });

        group.MapGet("/search", async Task<Results<Ok<List<OpportunityDTO>>, BadRequest<string>>> (MyJobContext db, HttpContext context) =>
        {
            var queryParameters = context.Request.Query
            .ToDictionary(k => k.Key, v => v.Value.ToString());

            try
            {
                var parameters =
                from opportunityPropertie in opportunityProperties
                from queryParameter in queryParameters
                where opportunityPropertie.Name == queryParameter.Key
                select new
                {
                    opportunityPropertie.Name,
                    Value = (dynamic)Convert.ChangeType(queryParameter.Value, opportunityPropertie.PropertyType)
                };

                StringBuilder sqlParameters = new();

                if(parameters.Count() > 2)
                    for (int i = 0; i < parameters.Count() -1; i++) 
                        sqlParameters.Append($"{parameters.First().Name} = {parameters.First().Value} and ");

                sqlParameters.Append($"{parameters.Last().Name} = {parameters.Last().Value}");

                var results = await db.Opportunities
                .FromSql($"SELECT * FROM Opportunities WHERE {sqlParameters}")
                .ToListAsync();

                return TypedResults.Ok(results
                    .Select(o => o.ToDTO(db))
                    .ToList());
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest($"{ex.Message} ,{ex.StackTrace}");
            }
        })
        .WithName("OpportunitiesSearch")
        .WithOpenApi();
    }

    private static void GetOpportunityByIdEndPoint(RouteGroupBuilder group)
    {
        group.MapGet("/{id}", async Task<Results<Ok<OpportunityDTO>, NotFound>> (int id, MyJobContext db) =>
        {
            return await db.Opportunities.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Opportunity model
                    ? TypedResults.Ok(model.ToDTO(db))
                    : TypedResults.NotFound();
        })
        .WithName("GetOpportunityById")
        .WithOpenApi();
    }

    private static void GetAllOpportunitiesEndPoint(RouteGroupBuilder group)
    {
        group.MapGet("/", async (MyJobContext db) =>
        {
            return await db.Opportunities
            .Select(o => o.ToDTO(db))
            .ToListAsync();
        })
        .WithName("GetAllOpportunities")
        .WithOpenApi();
    }
}
