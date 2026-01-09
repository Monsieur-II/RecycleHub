namespace RecycleHub.Pg.Sdk.Repositories.Interfaces;

public interface IPgRepository<T> where T : class
{
    Task<bool> AddAsync(T entity, bool saveChanges = false, CancellationToken ct = default);
    Task<T?> GetByIdAsync(string id, CancellationToken ct = default);
}
