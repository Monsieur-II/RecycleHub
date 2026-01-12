using Microsoft.EntityFrameworkCore;
using RecycleHub.Pg.Sdk.Entities;

namespace RecycleHub.Pg.Sdk.Repositories.Interfaces;

public interface IUnitOfWork
{
    public IPgRepository<RecycleCenter> RecycleCenters { get; }
    public IPgRepository<Material> Materials { get; }
    
    public DbContext GetDbContext();

    Task<bool> SaveChangesAsync();
}
