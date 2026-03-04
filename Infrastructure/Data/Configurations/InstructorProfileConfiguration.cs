using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class InstructorProfileConfiguration : IEntityTypeConfiguration<InstructorProfile>
{
    public void Configure(EntityTypeBuilder<InstructorProfile> builder)
    {
        builder.ToTable("instructors");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("instructor_id")
            .ValueGeneratedNever();

        builder.Property(x => x.Bio)
            .HasColumnName("bio");

        builder.Property(x => x.Expertise)
            .HasColumnName("expertise");

        builder.Property(x => x.VerificationStatus)
            .HasColumnName("verification_status")
            .HasConversion<string>()
            .HasDefaultValue(InstructorVerificationStatus.Pending);

        builder.Property(x => x.RatingAvg)
            .HasColumnName("rating_avg")
            .HasPrecision(3, 2)
            .HasDefaultValue(0m);

        builder.Property(x => x.RatingCount)
            .HasColumnName("rating_count")
            .HasDefaultValue(0);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()");

        builder.HasIndex(x => x.VerificationStatus)
            .HasDatabaseName("idx_instructors_verification");

        builder.HasIndex(x => x.RatingAvg)
            .HasDatabaseName("idx_instructors_rating");
    }
}
