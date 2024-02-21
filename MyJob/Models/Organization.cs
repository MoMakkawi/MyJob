using System;

using MyJob.DTOs;

namespace MyJob.Models;

public class Organization : User
{
    public virtual ICollection<Opportunity> Opportunities { get; set; } = [];

    public OrganizationDTO ToDTO()
        => new(Id,
               FirstName,
               Email,
               PhoneNumber,
               Specialty,
               About,
               Picture?.Path,
               Opportunities.Select(o => o.ToDTO()).ToList());
}
