using System.ComponentModel.DataAnnotations.Schema;

using MyJob.Database;
using MyJob.DTOs;

namespace MyJob.Models;

public class Opportunity
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public OpportunityType Type { get; set; }

    public string OrganizationFullName { get; set; } = string.Empty;
    public int? OrganizationId { get; set; }

    public int[] ApplicantsIds { get; set; } = [];

    [NotMapped]
    internal int MonthsNumber => GetMonthsNumber();

    private int GetMonthsNumber()
    {
        var endDateTime = DateTime.Parse(EndDate.ToString());
        var startDateTime = DateTime.Parse(StartDate.ToString());
        var daysNumber = endDateTime.Subtract(startDateTime).TotalDays;

        return (int)Math.Ceiling(daysNumber / 30);
    }

    public OpportunityQueryDTO ToDTO(MyJobContext db)
        => new (Id,
               Title,
               StartDate,
               EndDate,
               Type,
               MonthsNumber,
               ApplicantsIds,
               OrganizationId == null ? OrganizationFullName : db.Organizations
                    .FirstOrDefault(o => o.Id == OrganizationId)?.FullName!,
               OrganizationId);
    

}
