﻿using MyJob.Models;

namespace MyJob.DTOs.OpportunityDTO;

public record OpportunityQueryDTO(
    int Id,
    string Title,
    DateOnly StartDate,
    DateOnly EndDate,
    OpportunityType Type,
    int MonthsNumber,

    int[] ApplicantsCVIds,

    string OrganizationFullName,
    int? OrganizationId);