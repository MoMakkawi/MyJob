using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyJob.Models;

public class User
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
    [DataType(DataType.Password)]
    public required string Password { get; set; }
    [DataType(DataType.PhoneNumber)]
    public required string PhoneNumber { get; set; }
    public required string Specialty { get; set; }
    public required string About { get; set; }
    public virtual FileData? Picture { get; set; } = null!;
    public int? PictureId { get; set; }
}
