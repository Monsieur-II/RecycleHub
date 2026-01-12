using System.Linq.Expressions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using RecycleHub.Pg.Sdk.Entities;
using RecycleHub.Pg.Sdk.Repositories.Interfaces;

namespace RecycleHub.Pg.Sdk.Repositories.Providers;

public class PgRepository<T>(ApplicationDbContext dbContext) : IPgRepository<T>
    where T : class
{
    private IQueryable<T> GetBaseQuery()
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
    
    public virtual async Task<bool> UpdateAsync(T entity, bool saveChanges = false, CancellationToken ct = default)
    {
        dbContext.Update(entity);
        
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
    
    public virtual async Task<T?> GetByIdAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null, CancellationToken ct = default)
    {
        var query = GetBaseQuery();
        
        if (include != null)
        {
            query = include(query);
        }
        
        return await query
            .Where(predicate)
            .FirstOrDefaultAsync(ct);
    }

    public virtual async Task<List<TResponse>> GetPageAsync<TResponse>(PageFilter filter, Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
        where TResponse : class
    {
        var pred = predicate ?? (x => true);
        
        var query = await GetBaseQuery()
            .AsNoTracking()
                .Where(pred)
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ProjectToType<TResponse>()
                .ToListAsync(ct);
        
        return query;
    }
    
    public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
    {
        var pred = predicate ?? (x => true);
        
        var query = await GetBaseQuery()
            .AsNoTracking()
            .Where(pred)
            .ToListAsync(ct);
        
        return query;
    }

    public async Task<List<LookUpResponse>> GetLookupAsync(CancellationToken ct = default)
    {
        var res = await GetBaseQuery()
            .AsNoTracking()
            .ProjectToType<LookUpResponse>()
            .ToListAsync(ct);

        return res;
    }

    public virtual async Task<List<TResponse>> GetRecycleCentersAsync<TResponse>(PageFilter filter,
        Expression<Func<RecycleCenter, bool>>? predicate = null, CancellationToken ct = default) where TResponse : class

    {
        var pred = predicate ?? (x => true);

        var query = await dbContext.RecycleCenters
            .AsNoTracking()
            .Where(pred)
            .Skip((filter.PageIndex - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Include(x => x.Materials)
            .ProjectToType<TResponse>()
            .ToListAsync(ct);

        return query;
    }
}
