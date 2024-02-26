using MyJob.DTOs.OpportunityDTOs;

namespace MyJob.DTOs.OrganizationDTOs;

public record OrganizationQueryDTO(
    int Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string Specialty,
    string About,

    int? PictureId,
    string? PictureLink,

    List<OpportunityQueryDTO>? OpportunitiesDTOs);
