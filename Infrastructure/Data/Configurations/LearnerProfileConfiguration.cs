using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class LearnerProfileConfiguration : IEntityTypeConfiguration<LearnerProfile>
{
    public void Configure(EntityTypeBuilder<LearnerProfile> builder)
    {
        builder.ToTable("learners");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("learner_id")
            .ValueGeneratedNever();

        builder.Property(x => x.ParentId)
            .HasColumnName("parent_id");

        builder.Property(x => x.GradeLevel)
            .HasColumnName("grade_level");

        builder.Property(x => x.BirthYear)
            .HasColumnName("birth_year");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()");

        builder.HasIndex(x => x.ParentId)
            .HasDatabaseName("idx_learners_parent");
    }
}
