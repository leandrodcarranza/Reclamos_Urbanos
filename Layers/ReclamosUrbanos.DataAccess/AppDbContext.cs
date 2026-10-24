
using Microsoft.EntityFrameworkCore;
using ReclamosUrbanos.Entities;

namespace ReclamosUrbanos.DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Entidad de Franco
    public DbSet<Report> Reports => Set<Report>();

    // Entidades de Mauricio
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Neighborhood> Neighborhoods => Set<Neighborhood>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}
