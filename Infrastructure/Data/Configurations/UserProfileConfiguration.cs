using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("user_profiles");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("user_id")
            .ValueGeneratedNever();

        builder.Property(x => x.FullName)
            .HasColumnName("full_name");

        builder.Property(x => x.AvatarUrl)
            .HasColumnName("avatar_url");

        builder.Property(x => x.Phone)
            .HasColumnName("phone");

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasDefaultValue(UserStatus.Active);

        builder.Property(x => x.UserRole)
            .HasColumnName("user_role")
            .HasConversion<string>();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(x => x.UserRole)
            .HasDatabaseName("idx_user_profiles_role");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("idx_user_profiles_status");

        builder.HasOne<LearnerProfile>()
            .WithOne()
            .HasForeignKey<LearnerProfile>(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ParentProfile>()
            .WithOne()
            .HasForeignKey<ParentProfile>(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<InstructorProfile>()
            .WithOne()
            .HasForeignKey<InstructorProfile>(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<StaffProfile>()
            .WithOne()
            .HasForeignKey<StaffProfile>(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<AdminProfile>()
            .WithOne()
            .HasForeignKey<AdminProfile>(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
