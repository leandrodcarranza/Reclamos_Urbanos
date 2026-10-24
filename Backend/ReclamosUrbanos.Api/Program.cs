
using Microsoft.EntityFrameworkCore;
using ReclamosUrbanos.DataAccess;
using ReclamosUrbanos.DataAccess.Stores;
using ReclamosUrbanos.DataAccess.SqlStores;
using ReclamosUrbanos.BusinessLogic.Managers;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Falta la cadena de conexión 'DefaultConnection'.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Módulo Report - Franco
builder.Services.AddScoped<IReportStore, ReportStore>();
builder.Services.AddScoped<IReportManager, ReportManager>();

// Módulos Category y Neighborhood - Mauricio
builder.Services.AddScoped<ICategoryStore, CategoryStore>();
builder.Services.AddScoped<INeighborhoodStore, NeighborhoodStore>();

builder.Services.AddScoped<ICategoryManager, CategoryManager>();
builder.Services.AddScoped<INeighborhoodManager, NeighborhoodManager>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
