using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ParentProfileConfiguration : IEntityTypeConfiguration<ParentProfile>
{
    public void Configure(EntityTypeBuilder<ParentProfile> builder)
    {
        builder.ToTable("parents");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("parent_id")
            .ValueGeneratedNever();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()");
    }
}
