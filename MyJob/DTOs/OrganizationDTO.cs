namespace MyJob.DTOs;

public record OrganizationDTO(
    int Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string Specialty,
    string About,
    string? PictureLink,

    List<OpportunityDTO>? OpportunitiesDTOs);
