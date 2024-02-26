using MyJob.Models;

namespace MyJob.DTOs.OpportunityDTOs;

public record OpportunityCommandDTO(
    string Title,
    DateOnly StartDate,
    DateOnly EndDate,
    OpportunityType Type,

    string? OrganizationFullName,
    int? OrganizationId);
