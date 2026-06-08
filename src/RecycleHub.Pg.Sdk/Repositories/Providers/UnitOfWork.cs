using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecycleHub.Pg.Sdk.Entities;
using RecycleHub.Pg.Sdk.Repositories.Interfaces;

namespace RecycleHub.Pg.Sdk.Repositories.Providers;

public class UnitOfWork(IServiceProvider serviceProvider) : IUnitOfWork
{
    private IPgRepository<RecycleCenter>? _recycleCentersRepo;
    private IPgRepository<Material>? _materialsRepo;
    private IPgRepository<Review>? _reviewsRepo;
    private IPgRepository<Photo>? _photosRepo;
    private IPgRepository<Favorite>? _favoritesRepo;

    public IPgRepository<RecycleCenter> RecycleCenters => _recycleCentersRepo ??= serviceProvider.GetRequiredService<IPgRepository<RecycleCenter>>();
    public IPgRepository<Material> Materials => _materialsRepo ??= serviceProvider.GetRequiredService<IPgRepository<Material>>();
    public IPgRepository<Review> Reviews => _reviewsRepo ??= serviceProvider.GetRequiredService<IPgRepository<Review>>();
    public IPgRepository<Photo> Photos => _photosRepo ??= serviceProvider.GetRequiredService<IPgRepository<Photo>>();
    public IPgRepository<Favorite> Favorites => _favoritesRepo ??= serviceProvider.GetRequiredService<IPgRepository<Favorite>>();
    
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
