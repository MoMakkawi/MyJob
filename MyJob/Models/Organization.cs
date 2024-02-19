namespace MyJob.Models;

public class Organization : User
{
    public virtual ICollection<Opportunity> Opportunities { get; set; } = [];
}
