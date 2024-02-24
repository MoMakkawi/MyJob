namespace MyJob.DTOs.OpportunitySeekerDTOs;

public record OpportunitySeekerCommandDTO(
    string FirstName, 
    string LastName,
    string Email,
    string Password,
    string PhoneNumber,
    string Specialty,
    string About,
    int[]? ExperiencesIds,
    int? PictureId,
    int? CVId);
