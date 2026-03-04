using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class AdminProfileConfiguration : IEntityTypeConfiguration<AdminProfile>
{
    public void Configure(EntityTypeBuilder<AdminProfile> builder)
    {
        builder.ToTable("admins");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("admin_id")
            .ValueGeneratedNever();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()");
    }
}
