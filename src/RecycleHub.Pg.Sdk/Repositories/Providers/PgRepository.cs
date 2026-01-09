using RecycleHub.Pg.Sdk.Repositories.Interfaces;

namespace RecycleHub.Pg.Sdk.Repositories.Providers;

public class PgRepository<T>(ApplicationDbContext dbContext) : IPgRepository<T>
    where T : class
{
    protected IQueryable<T> GetBaseQuery()
    {
        return dbContext.Set<T>();
    }
    
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await dbContext.SaveChangesAsync(ct);
    }
    
    public virtual async Task<bool> AddAsync(T entity, bool saveChanges = false, CancellationToken ct = default)
    {
        _ =  await dbContext.AddAsync(entity, ct);
        
        if (saveChanges)
        {
            await dbContext.SaveChangesAsync(ct);
        }
        
        return true;
    }
    
    public virtual async Task<T?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        return await dbContext.Set<T>().FindAsync([id], ct);
    }
}
