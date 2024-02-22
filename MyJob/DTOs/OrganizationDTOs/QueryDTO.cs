namespace MyJob.DTOs.OrganizationDTOs;

public record QueryDTO(
    int Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string Specialty,
    string About,
    string? PictureLink,

    List<OpportunityDTO>? OpportunitiesDTOs);
