namespace MyJob.DTOs.OrganizationDTOs;

public record OrganizationCommandDTO(
    string FullName,
    string Email,
    string Password,
    string PhoneNumber,
    string Specialty,
    string About,
    int? PictureId);

    