using System.ComponentModel.DataAnnotations.Schema;

using MyJob.DTOs;

namespace MyJob.Models;

public class Opportunity
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public OpportunityType Type { get; set; }

    [NotMapped]
    internal int MonthsNumber => GetMonthsNumber();

    public virtual Organization? Organization { get; set; } = null!;
    public int? OrganizationId { get; set; }


    private int GetMonthsNumber()
    {
        var endDateTime = DateTime.Parse(EndDate.ToString());
        var startDateTime = DateTime.Parse(StartDate.ToString());
        var daysNumber = endDateTime.Subtract(startDateTime).TotalDays;

        return (int)Math.Ceiling(daysNumber / 30);
    }

    public OpportunityDTO ToDTO()
        => new(Id,
               Title,
               StartDate,
               EndDate,
               Type,
               MonthsNumber,
               Organization?.ToDTO());
    

}
