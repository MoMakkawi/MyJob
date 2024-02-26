using MyJob.DTOs.OpportunityDTOs;

namespace MyJob.DTOs;

public record OpportunitySeekerQueryDTO(
    int Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string Specialty,
    string About,

    int? PictureId,
    string? PictureLink,

    int PracticalExperienceMonthsNumber,
    int VolunteerExperienceMonthsNumber,

    List<OpportunityQueryDTO>? ExperiencesDTOs,

    int? CVId,
    string? CVLink);
