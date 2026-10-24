
using Microsoft.EntityFrameworkCore;
using ReclamosUrbanos.DataAccess;
using ReclamosUrbanos.DataAccess.Stores;
using ReclamosUrbanos.DataAccess.SqlStores;
using ReclamosUrbanos.BusinessLogic.Managers;
using System.Text.Json.Serialization;

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

// CORS: permite que el front de Next.js (en desarrollo) llame a la API
const string FrontendCorsPolicy = "Frontend";

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
        policy.WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Los enums (Status, Priority) viajan como texto en lugar de número
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(FrontendCorsPolicy);

app.MapControllers();

app.Run();
