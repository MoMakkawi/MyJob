using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using MyJob.Database;
using MyJob.Models;
namespace MyJob.EndPoints;

public static class FileDataEndpoints
{
    public static void MapFileDataEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/FileData").WithTags(nameof(FileData));

        GetAllFileDataEndPoint(group);
        GetFileDataByIdEndPoint(group);
        UpdateFileDataEndPoint(group);
        CreateFileDataEndPoint(group);
        DeleteFileDataEndPoint(group);
    }

    private static void DeleteFileDataEndPoint(RouteGroupBuilder group)
    {
        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, MyJobContext db) =>
        {
            await db.Files
                .Where(model => model.Id == id)
                .ForEachAsync(file => File.Delete(file.Path));

            var affected = await db.Files
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteFileData")
        .WithOpenApi();
    }

    private static void CreateFileDataEndPoint(RouteGroupBuilder group)
    {
        group.MapPost("/", async (IFormFile formFile, MyJobContext db) =>
        {
            var fileData = new FileData(formFile.ContentType, formFile.FileName, formFile);

            using var stream = new FileStream(fileData.Path, FileMode.Create);
            await formFile.CopyToAsync(stream);

            db.Files.Add(fileData);
            await db.SaveChangesAsync();
            return TypedResults.Created(fileData.Path, fileData);
        })
        .Accepts<IFormFile>("multipart/form-data")
        .Produces(200)
        .WithName("CreateFileData")
        .WithOpenApi();


    }

    private static void UpdateFileDataEndPoint(RouteGroupBuilder group)
    {
        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, IFormFile formFile, MyJobContext db) =>
        {
            var file = await db.Files.SingleAsync(model => model.Id == id);
            string filePath = file.Path;

            var affected = await db.Files
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.ContentType, formFile.ContentType)
                    .SetProperty(m => m.Name, formFile.Name)
                    );

            File.Delete(filePath);


            var updatedFileData = new FileData(formFile.ContentType, formFile.Name);
            using var stream = new FileStream(updatedFileData.Path, FileMode.Create);
            await updatedFileData.FormFile.CopyToAsync(stream);

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateFileData")
        .WithOpenApi();
    }

    private static void GetFileDataByIdEndPoint(RouteGroupBuilder group)
    {
        group.MapGet("/{id}", async Task<Results<Ok<FileData>, NotFound>> (int id, MyJobContext db) =>
        {
            return await db.Files.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is FileData model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetFileDataById")
        .WithOpenApi();
    }

    private static void GetAllFileDataEndPoint(RouteGroupBuilder group)
    {
        group.MapGet("/", async (MyJobContext db) => await db.Files.ToListAsync())
        .WithName("GetAllFileData")
        .WithOpenApi();
    }
}
