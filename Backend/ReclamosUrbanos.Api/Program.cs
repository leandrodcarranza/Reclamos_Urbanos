using Microsoft.EntityFrameworkCore;
using ReclamosUrbanos.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// --- Base de datos ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'DefaultConnection'.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- Stores (cada integrante registra los suyos) ---
// builder.Services.AddScoped<IReportStore, ReportStore>();

// --- Managers (cada integrante registra los suyos) ---
// builder.Services.AddScoped<IReportManager, ReportManager>();

// --- Servicio de IA ---
// builder.Services.AddScoped<IAiService, GeminiAiService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
