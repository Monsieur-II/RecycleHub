using System.Linq.Expressions;
using RecycleHub.Pg.Sdk.Entities;

namespace RecycleHub.Pg.Sdk.Repositories.Interfaces;

public interface IPgRepository<T> where T : class
{
    Task<bool> AddAsync(T entity, bool saveChanges = false, CancellationToken ct = default);
    Task<bool> UpdateAsync(T entity, bool saveChanges = false, CancellationToken ct = default);
    
    Task<T?> GetByIdAsync(string id, CancellationToken ct = default);

    Task<T?> GetByIdAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null,
        CancellationToken ct = default);

    Task<List<TResponse>> GetRecycleCentersAsync<TResponse>(PageFilter filter,
        Expression<Func<RecycleCenter, bool>>? predicate = null, CancellationToken ct = default)
        where TResponse : class;

    Task<List<TResponse>> GetPageAsync<TResponse>(PageFilter filter, Expression<Func<T, bool>>? predicate = null,
        CancellationToken ct = default)
        where TResponse : class;

    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,
        CancellationToken ct = default);

    Task<List<LookUpResponse>> GetLookupAsync(CancellationToken ct = default);
}
