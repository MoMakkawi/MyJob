using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using MyJob.Database;
using MyJob.Models;
using MyJob.DTOs.OrganizationDTOs;
namespace MyJob.EndPoints;

public static class OrganizationEndpoints
{
    public static void MapOrganizationEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Organization").WithTags(nameof(Organization));

        SearchOrganizationsEndPoint(group);
        UpdateOrganizationEndPoint(group);
        CreateOrganizationEndPoint(group);
        DeleteOrganizationEndPoint(group);
    }
    private static void CreateOrganizationEndPoint(RouteGroupBuilder group)
    {
        group.MapPost("/", async (CommandDTO createOrganizationDTO, MyJobContext db) =>
        {
            var picture = await db.Files
            .FindAsync(createOrganizationDTO.PictureId);

            var organization = new Organization()
            {
                FirstName = createOrganizationDTO.FirstName,
                LastName = createOrganizationDTO.LastName,
                Email = createOrganizationDTO.Email,
                About = createOrganizationDTO.About,
                Password = createOrganizationDTO.Password,
                PhoneNumber = createOrganizationDTO.PhoneNumber,
                Specialty = createOrganizationDTO.Specialty,
                Picture = picture
            };

            db.Organizations.Add(organization);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Organization/{organization.Id}", organization);
        })
        .WithName("RegisterOrganization")
        .WithOpenApi();
    }

    private static void UpdateOrganizationEndPoint(RouteGroupBuilder group)
    {
        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, CommandDTO organizationDTO, MyJobContext db) =>
        {

            var organizationPicture = db.Organizations
                .SingleOrDefault(model => model.Id == id)?
                .Picture;
            int? organizationPictureId = organizationPicture?.Id;

            var affected = await db.Organizations
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.FirstName, organizationDTO.FirstName)
                    .SetProperty(m => m.LastName, organizationDTO.LastName)
                    .SetProperty(m => m.Email, organizationDTO.Email)
                    .SetProperty(m => m.Password, organizationDTO.Password)
                    .SetProperty(m => m.PhoneNumber, organizationDTO.PhoneNumber)
                    .SetProperty(m => m.Specialty, organizationDTO.Specialty)
                    .SetProperty(m => m.About, organizationDTO.About)
                    .SetProperty(m => m.PictureId, organizationPictureId)
                    );

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateOrganization")
        .WithOpenApi();
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

    private static void SearchOrganizationsEndPoint(RouteGroupBuilder group)
    {
        group.MapGet("/", (
            int? Id,
            string? FullName,
            string? Email,
            string? PhoneNumber,
            string? Specialty,
            MyJobContext db) =>
        {
            var organizations = db.Organizations.AsEnumerable();

            if(Id is not null)
                organizations = organizations.Where(o => o.Id == Id);

            if (FullName is not null)
                organizations = organizations.Where(o => o.FullName.Contains(FullName));

            if (Email is not null)
                organizations = organizations.Where(o => o.Email == Email);

            if (Specialty is not null)
                organizations = organizations.Where(o => o.Specialty == Specialty);

            if (PhoneNumber is not null)
                organizations = organizations.Where(o => o.PhoneNumber == PhoneNumber);

            return organizations.Select(model => model.ToDTO(db));
        })
        .WithName("SearchOrganizations")
        .WithOpenApi();
    }

}