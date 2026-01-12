using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecycleHub.Pg.Sdk.Entities;
using RecycleHub.Pg.Sdk.Repositories.Interfaces;

namespace RecycleHub.Pg.Sdk.Repositories.Providers;

public class UnitOfWork(IServiceProvider serviceProvider) : IUnitOfWork
{
    private IPgRepository<RecycleCenter>? _recycleCentersRepo;
    private IPgRepository<Material>? _materialsRepo;

    public IPgRepository<RecycleCenter> RecycleCenters => _recycleCentersRepo ??= serviceProvider.GetRequiredService<IPgRepository<RecycleCenter>>();
    public IPgRepository<Material> Materials => _materialsRepo ??= serviceProvider.GetRequiredService<IPgRepository<Material>>();
    
    public async Task<bool> SaveChangesAsync()
    {
        return await serviceProvider.GetRequiredService<ApplicationDbContext>()
            .SaveChangesAsync() > 0;
    }
    
    public DbContext GetDbContext()
    {
        return serviceProvider.GetRequiredService<ApplicationDbContext>();
    }
}
