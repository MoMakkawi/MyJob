using Microsoft.EntityFrameworkCore;

using MyJob.Models;

namespace MyJob.Database;

public class MyJobContext(DbContextOptions<MyJobContext> options) : DbContext(options)
{
    public DbSet<FileData> Files { get; set; }
    public DbSet<Opportunity> Opportunities { get; set; }
    public DbSet<OpportunitySeeker> OpportunitySeekers { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .UseTpcMappingStrategy();

        modelBuilder.Entity<Organization>()
            .HasMany(o => o.Opportunities)
            .WithOne()
            .HasForeignKey(x => x.OrganizationId);

        modelBuilder.Entity<OpportunitySeeker>()
            .HasMany(s => s.Experiences);


        base.OnModelCreating(modelBuilder);
    }
}
