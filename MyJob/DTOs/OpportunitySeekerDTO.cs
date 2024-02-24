namespace MyJob.DTOs;

public record OpportunitySeekerDTO(
    int Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string Specialty,
    string About,
    string? PictureLink,

    int PracticalExperienceMonthsNumber,
    int VolunteerExperienceMonthsNumber,
    List<OpportunityDTO.OpportunityQueryDTO>? ExperiencesDTOs,
    int? CVId,
    string? CVLink);
