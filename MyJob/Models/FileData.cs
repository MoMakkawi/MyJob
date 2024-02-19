using System.ComponentModel.DataAnnotations.Schema;

namespace MyJob.Models;

public class FileData
{
    public int Id { get; set; }
    public string ContentType { get; set; }
    public string Name { get; set; }

    [NotMapped]
    internal string Path => GetPath();
    [NotMapped]
    internal IFormFile FormFile { get; set; }

    public FileData(string contentType, string name)
    {
        ContentType = contentType;
        Name = name;
        FormFile = default!;
    }

    public FileData(int id, string contentType, string name, IFormFile formFile)
    : this(contentType, name)
    {
        Id = id;
        FormFile = formFile;
    }

    public FileData(string contentType, string name, IFormFile formFile)
    {
        ContentType = contentType;
        Name = name;
        FormFile = formFile;
    }


    private string GetPath()
    {
        string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files");

        //create folder if not exist
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        //get file extension
        FileInfo fileInfo = new(Name);
        string fileName = string.Concat(Id, fileInfo.Extension);

        return System.IO.Path.Combine(path, fileName);
    }
}