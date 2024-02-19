namespace MyJob.Models;

public sealed class Organization : User
{
    public ICollection<Opportunity> Opportunities { get; set; } = [];
}
