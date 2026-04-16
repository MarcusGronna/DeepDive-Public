using InfraSprint.Api.Data;
using InfraSprint.Api.Models;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

Directory.CreateDirectory("Data");

var missionsCreateCounter = Metrics.CreateCounter(
    "missions_created_total",
    "Total number of missions created."
);

app.UseHttpMetrics();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    app = "MissionBoard",
    timestamp = DateTime.UtcNow
}));

app.MapGet("/missions", async (AppDbContext db) =>
{
    var missions = await db.Missions
    .OrderByDescending(m => m.CreatedAt)
    .ToListAsync();

    return Results.Ok(missions);
});

app.MapPost("/missions", async (CreateMissionRequest request, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest("Title is required.");
    }

    if (string.IsNullOrWhiteSpace(request.Region))
    {
        return Results.BadRequest("Region is required.");
    }

    if (request.ThreatLevel < 1 || request.ThreatLevel > 5)
    {
        return Results.BadRequest("Treat level must be between 1 and 5.");
    }

    var mission = new Mission
    {
        Title = request.Title.Trim(),
        Region = request.Region.Trim(),
        ThreatLevel = request.ThreatLevel,
        CreatedAt = DateTime.UtcNow
    };

    db.Missions.Add(mission);
    await db.SaveChangesAsync();

    missionsCreateCounter.Inc();

    return Results.Created($"/missions/{mission.Id}", mission);
});

app.MapMetrics();

app.UseHttpsRedirection();

app.Run();

public partial class Program { }
public record CreateMissionRequest(string Title, string Region, int ThreatLevel);
