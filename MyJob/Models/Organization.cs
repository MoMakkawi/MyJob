using MyJob.Database;
using MyJob.DTOs.OrganizationDTOs;

namespace MyJob.Models;

public class Organization : User
{ 
    public virtual ICollection<Opportunity> Opportunities { get; set; } = [];

    public OrganizationQueryDTO ToDTO(MyJobContext db)
        => new(Id,
               FullName,
               Email,
               PhoneNumber,
               Specialty,
               About,
               Picture?.Id,
               Picture?.Path,
               Opportunities.Select(o => o.ToDTO(db)).ToList());
}
