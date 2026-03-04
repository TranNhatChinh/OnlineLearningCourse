using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext, IAppDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<LearnerProfile> LearnerProfiles { get; set; }
    public DbSet<ParentProfile> ParentProfiles { get; set; }
    public DbSet<InstructorProfile> InstructorProfiles { get; set; }
    public DbSet<StaffProfile> StaffProfiles { get; set; }
    public DbSet<AdminProfile> AdminProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}