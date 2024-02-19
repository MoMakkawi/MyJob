using System.ComponentModel.DataAnnotations.Schema;

namespace MyJob.Models;

public class OpportunitySeeker : User
{
    [NotMapped]
    internal int PracticalExperienceMonthsNumber => Experiences
           .Aggregate(0, (total, experience) =>
               experience.Type == OpportunityType.Work ? total + experience.MonthsNumber : total);
    [NotMapped]
    internal int PracticalVolunteerMonthsNumber => Experiences
           .Aggregate(0, (total, experience) =>
               experience.Type == OpportunityType.volunteer ? total + experience.MonthsNumber : total);
    public virtual ICollection<Opportunity> Experiences { get; set; } = [];
    public virtual FileData? CV { get; set; } = null!;
    public int? CVId { get; set; }
}
