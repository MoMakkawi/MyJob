namespace MyJob.DTOs.OrganizationDTOs;

public record OrganizationCommandDTO(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string PhoneNumber,
    string Specialty,
    string About,
    int? PictureId);

    