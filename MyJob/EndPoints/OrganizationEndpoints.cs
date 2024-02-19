using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using MyJob.Database;
using MyJob.Models;
namespace MyJob.EndPoints;

public static class OrganizationEndpoints
{
    public static void MapOrganizationEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Organization").WithTags(nameof(Organization));

        GetAllOrganizationsEndPoint(group);
        GetOrganizationByIdEndPoint(group);
        UpdateOrganizationEndPoint(group);
        CreateOrganizationEndPoint(group);
        DeleteOrganizationEndPoint(group);
    }

    private static void DeleteOrganizationEndPoint(RouteGroupBuilder group)
    {
        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, MyJobContext db) =>
        {
            var affected = await db.Organizations
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteOrganization")
        .WithOpenApi();
    }

    private static void CreateOrganizationEndPoint(RouteGroupBuilder group)
    {
        group.MapPost("/", async (Organization organization, MyJobContext db) =>
        {
            db.Organizations.Add(organization);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Organization/{organization.Id}", organization);
        })
        .WithName("CreateOrganization")
        .WithOpenApi();
    }

    private static void UpdateOrganizationEndPoint(RouteGroupBuilder group)
    {
        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Organization organization, MyJobContext db) =>
        {
            var affected = await db.Organizations
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, organization.Id)
                    .SetProperty(m => m.FirstName, organization.FirstName)
                    .SetProperty(m => m.LastName, organization.LastName)
                    .SetProperty(m => m.Email, organization.Email)
                    .SetProperty(m => m.Password, organization.Password)
                    .SetProperty(m => m.PhoneNumber, organization.PhoneNumber)
                    .SetProperty(m => m.Specialty, organization.Specialty)
                    .SetProperty(m => m.About, organization.About)
                    .SetProperty(m => m.PictureId, organization.PictureId)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateOrganization")
        .WithOpenApi();
    }

    private static void GetOrganizationByIdEndPoint(RouteGroupBuilder group)
    {
        group.MapGet("/{id}", async Task<Results<Ok<Organization>, NotFound>> (int id, MyJobContext db) =>
        {
            return await db.Organizations.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Organization model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetOrganizationById")
        .WithOpenApi();
    }

    private static void GetAllOrganizationsEndPoint(RouteGroupBuilder group)
    {
        group.MapGet("/", async (MyJobContext db) =>
        {
            return await db.Organizations.ToListAsync();
        })
        .WithName("GetAllOrganizations")
        .WithOpenApi();
    }
}
