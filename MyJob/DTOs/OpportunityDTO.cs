using MyJob.Models;

namespace MyJob.DTOs;

public record OpportunityDTO(
    int Id,
    string Title,
    DateOnly StartDate,
    DateOnly EndDate,
    OpportunityType Type,
    int MonthsNumber,
    OrganizationDTO? OrganizationDTO);
