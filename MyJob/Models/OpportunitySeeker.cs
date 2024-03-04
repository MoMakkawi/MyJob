using System.ComponentModel.DataAnnotations.Schema;

using MyJob.Database;
using MyJob.DTOs;

namespace MyJob.Models;

public class OpportunitySeeker : User
{

    [NotMapped]
    internal int PracticalExperienceMonthsNumber => Experiences
           .Aggregate(0, (total, experience) =>
               experience.Type == OpportunityType.Work ? total + experience.MonthsNumber : total);

    [NotMapped]
    internal int VolunteerExperienceMonthsNumber => Experiences
           .Aggregate(0, (total, experience) =>
               experience.Type == OpportunityType.Volunteer ? total + experience.MonthsNumber : total);
    public virtual ICollection<Opportunity> Experiences { get; set; } = [];
    public virtual FileData? CV { get; set; } = null!;
    public int? CVId { get; set; }

    public OpportunitySeekerQueryDTO ToDTO(MyJobContext db)
        => new(Id,
               FullName,
               Email,
               PhoneNumber,
               Specialty,
               About,
               Picture?.Id,
               Picture?.Path,
               PracticalExperienceMonthsNumber,
               VolunteerExperienceMonthsNumber,
               Experiences.Select(o => o.ToDTO(db)).ToList(),
               CV?.Id,
               CV?.Path);
}
