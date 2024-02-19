using Microsoft.EntityFrameworkCore;

using MyJob.Models;

namespace MyJob.Database;

public class MyJobContext : DbContext
{
    public DbSet<FileData> Files { get; set; }
    public DbSet<Opportunity> Opportunities { get; set; }
    public DbSet<OpportunitySeeker> OpportunitySeekers { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<User> Users { get; set; }

    public MyJobContext(DbContextOptions<MyJobContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);
    }
}
