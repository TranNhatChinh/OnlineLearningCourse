using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions;

public interface IAppDbContext
{
    DbSet<UserProfile> UserProfiles { get; }
    DbSet<LearnerProfile> LearnerProfiles { get; }
    DbSet<ParentProfile> ParentProfiles { get; }
    DbSet<InstructorProfile> InstructorProfiles { get; }
    DbSet<StaffProfile> StaffProfiles { get; }
    DbSet<AdminProfile> AdminProfiles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
