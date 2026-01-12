using Microsoft.EntityFrameworkCore;
using RecycleHub.Pg.Sdk.Entities;

namespace RecycleHub.Pg.Sdk;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<RecycleCenter> RecycleCenters { get; set; }
    public DbSet<Material> Materials { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
