using Microsoft.EntityFrameworkCore;
using RecycleHub.Pg.Sdk.Entities;

namespace RecycleHub.Pg.Sdk.Repositories.Interfaces;

public interface IUnitOfWork
{
    public IPgRepository<RecycleCenter> RecycleCenters { get; }
    public IPgRepository<Material> Materials { get; }
    public IPgRepository<Review> Reviews { get; }
    public IPgRepository<Photo> Photos { get; }
    public IPgRepository<Favorite> Favorites { get; }

    public DbContext GetDbContext();

    Task<bool> SaveChangesAsync();
}
