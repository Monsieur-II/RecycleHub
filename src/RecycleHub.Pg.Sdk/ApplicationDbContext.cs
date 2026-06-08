using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecycleHub.Pg.Sdk.Entities;

namespace RecycleHub.Pg.Sdk;

public class ApplicationDbContext(DbContextOptions options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<RecycleCenter> RecycleCenters { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Favorite> Favorites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
