using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using MyJob.Database;
using MyJob.Models;
using MyJob.DTOs;
namespace MyJob.EndPoints;

public static class OpportunitySeekerEndpoints
{
    public static void MapOpportunitySeekerEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/OpportunitySeeker").WithTags(nameof(OpportunitySeeker));

        GetAllOpportunitySeekersEndPoint(group);
        GetOpportunitySeekerByIdEndPoint(group);
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
        .WithName("CreateOpportunitySeeker")
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

    private static void GetOpportunitySeekerByIdEndPoint(RouteGroupBuilder group)
    {
        group.MapGet("/{id}", async Task<Results<Ok<OpportunitySeekerDTO>, NotFound>> (int id, MyJobContext db) =>
        {
            return await db.OpportunitySeekers.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is not OpportunitySeeker model ? TypedResults.NotFound()
                    : TypedResults.Ok(model.ToDTO(db));
        })
        .WithName("GetOpportunitySeekerById")
        .WithOpenApi();
    }

    private static void GetAllOpportunitySeekersEndPoint(RouteGroupBuilder group)
    {
        group.MapGet("/", async (MyJobContext db) =>
        {
            return await db.OpportunitySeekers
            .Select(os => os.ToDTO(db))
            .ToListAsync();
        })
        .WithName("GetAllOpportunitySeekers")
        .WithOpenApi();
    }
}
