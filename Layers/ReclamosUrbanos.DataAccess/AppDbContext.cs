using Microsoft.EntityFrameworkCore;

namespace ReclamosUrbanos.DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Cada integrante agrega su DbSet cuando se mergea su PR. Por ejemplo:
    // public DbSet<Category> Categories => Set<Category>();
    // public DbSet<Neighborhood> Neighborhoods => Set<Neighborhood>();
    // public DbSet<Report> Reports => Set<Report>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Carga automáticamente las clases IEntityTypeConfiguration<T> de este proyecto
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
