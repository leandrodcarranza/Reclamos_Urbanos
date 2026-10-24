
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReclamosUrbanos.Entities;

namespace ReclamosUrbanos.DataAccess.Configurations;

public class CategoryConfiguration
    : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.HasIndex(c => c.Name)
            .IsUnique();

        // Datos iniciales de las categorías
        builder.HasData(
            new Category
            {
                Id = 1,
                Name = "Bache",
                Description = "Problemas en el pavimento"
            },
            new Category
            {
                Id = 2,
                Name = "Alumbrado",
                Description = "Problemas de iluminación pública"
            },
            new Category
            {
                Id = 3,
                Name = "Basura",
                Description = "Problemas de recolección de residuos"
            }
        );
    }
}