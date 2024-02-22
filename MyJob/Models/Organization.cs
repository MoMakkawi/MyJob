using MyJob.Database;
using MyJob.DTOs.OrganizationDTOs;

namespace MyJob.Models;

public class Organization : User
{
    public virtual ICollection<Opportunity> Opportunities { get; set; } = [];

    public QueryDTO ToDTO(MyJobContext db)
        => new(Id,
               FirstName,
               Email,
               PhoneNumber,
               Specialty,
               About,
               Picture?.Path,
               Opportunities.Select(o => o.ToDTO(db)).ToList());
}
