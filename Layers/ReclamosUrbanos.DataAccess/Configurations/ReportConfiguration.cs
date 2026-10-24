
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReclamosUrbanos.Entities;

namespace ReclamosUrbanos.DataAccess.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("Reports");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Description)
            .IsRequired();

        builder.Property(r => r.Summary);

        builder.Property(r => r.Status)
            .IsRequired();

        builder.Property(r => r.Priority)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.HasOne(r => r.Category)
            .WithMany()
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Neighborhood)
            .WithMany()
            .HasForeignKey(r => r.NeighborhoodId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
