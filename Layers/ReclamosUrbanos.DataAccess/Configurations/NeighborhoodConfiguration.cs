
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReclamosUrbanos.Entities;

namespace ReclamosUrbanos.DataAccess.Configurations;

public class NeighborhoodConfiguration
    : IEntityTypeConfiguration<Neighborhood>
{
    public void Configure(EntityTypeBuilder<Neighborhood> builder)
    {
        builder.ToTable("Neighborhoods");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(n => n.Name)
            .IsUnique();

        // Datos iniciales de los barrios
        builder.HasData(
            new Neighborhood
            {
                Id = 1,
                Name = "Alta Córdoba"
            },
            new Neighborhood
            {
                Id = 2,
                Name = "Nueva Córdoba"
            },
            new Neighborhood
            {
                Id = 3,
                Name = "General Paz"
            },
            new Neighborhood
            {
                Id = 4,
                Name = "Alberdi"
            }
        );
    }
}
