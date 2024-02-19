using System.ComponentModel.DataAnnotations.Schema;

namespace MyJob.Models;

public class User
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    [NotMapped]
    internal string FullName => FirstName + " " + LastName;
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Specialty { get; set; }
    public required string About { get; set; }


    public FileData? Picture { get; set; } = null!;
    public int PictureId { get; set; }
}
