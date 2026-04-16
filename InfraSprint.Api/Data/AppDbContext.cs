using InfraSprint.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InfraSprint.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Mission> Missions => Set<Mission>();
}
