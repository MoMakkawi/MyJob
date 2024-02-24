using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using MyJob.Database;
using MyJob.Models;
using MyJob.DTOs;
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
        GetFileDataByIdsEndPoint(group);
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
        group.MapPost("/", async Task<Results<Ok<string>, BadRequest>> (HttpRequest request, MyJobContext db) =>
        {
            if (!request.HasFormContentType) return TypedResults.BadRequest();
            IFormFile? formFile = request.Form.Files.Single();
            if (formFile is null || formFile.Length == 0) return TypedResults.BadRequest();

            var fileData = new FileData(formFile.ContentType, formFile.FileName, formFile);

            db.Files.Add(fileData);
            await db.SaveChangesAsync();

            // Create the file on the server
            await using var fileStream = File.Create(fileData.Path);
            // Copy the file content to the file stream
            await formFile.CopyToAsync(fileStream);

            return TypedResults.Ok($"File {fileData.Name} saved successfully.");
        })
        .Accepts<IFormFile>("multipart/form-data")
        .Produces(200)
        .WithName("UploadFile")
        .WithSummary("Use postman to upload the file")
        .WithOpenApi();
    }

    private static void UpdateFileDataEndPoint(RouteGroupBuilder group)
    {
        group.MapPut("/{id}", async Task<Results<Ok, NotFound,BadRequest>> (int id, HttpRequest request, MyJobContext db) =>
        {
            if (!request.HasFormContentType) return TypedResults.BadRequest();
                var formFile = request.Form.Files.Single();

            if (formFile is null || formFile.Length == 0) return TypedResults.BadRequest();

            var file = await db.Files.SingleAsync(model => model.Id == id);

            var affected = await db.Files
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.ContentType, formFile.ContentType)
                    .SetProperty(m => m.Name, formFile.Name)
                    );

            File.Delete(file.Path);

            file.FormFile = formFile;
            file.ContentType = formFile.ContentType;
            file.Name = formFile.Name;

            using var stream = new FileStream(file.Path, FileMode.Create);
            await file.FormFile.CopyToAsync(stream);

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateFileData")
        .WithSummary("Use postman to update file")
        .WithOpenApi();
    }

    private static void GetFileDataByIdEndPoint(RouteGroupBuilder group)
    {
        group.MapGet("/{id}", async Task<Results<Ok<FileDataDTO>, NotFound>> (int id, MyJobContext db) =>
        {
            return await db.Files.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is not FileData model || !File.Exists(model.Path) 
                ? TypedResults.NotFound()
                : TypedResults.Ok(new FileDataDTO(
                    model.Id,
                    model.Name,
                    model.ContentType,
                    model.Path));
            
        })
        .WithName("GetFileDataById")
        .WithOpenApi();
    }
    private static void GetFileDataByIdsEndPoint(RouteGroupBuilder group)
    {
        group.MapGet("/Ids", (int[] ids, MyJobContext db) =>
            from id in ids
            from fileData in db.Files
            where fileData.Id == id
            select new FileDataDTO(
                fileData.Id,
                fileData.Name,
                fileData.ContentType,
                fileData.Path))
        .WithName("GetFileDataByIds")
        .WithOpenApi();
    }
    private static void GetAllFileDataEndPoint(RouteGroupBuilder group)
    {
        group.MapGet("/", async (MyJobContext db) =>
        {
            var Files = await db.Files
            .ToListAsync();

            var ExistFiles = Files
            .Where(fileData => File.Exists(fileData.Path));

            return ExistFiles
            .Select(fileData => new FileDataDTO(
                fileData.Id,
                fileData.Name,
                fileData.ContentType,
                fileData.Path));

        }).WithName("GetAllFileData")
        .WithOpenApi();
        
    }
}
