﻿namespace MyJob.DTOs.OrganizationDTOs;

public record OrganizationCommandDTO(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string PhoneNumber,
    string Specialty,
    string About,
    int? PictureId);

    