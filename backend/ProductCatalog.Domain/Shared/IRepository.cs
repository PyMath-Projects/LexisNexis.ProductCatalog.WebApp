namespace ProductCatalog.Domain.Shared;

public interface IRepository<TEntity, in TId>
    where TEntity : AggregateRoot<TId>
    where TId : notnull
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default);

    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct = default);

    Task AddAsync(TEntity entity, CancellationToken ct = default);

    Task UpdateAsync(TEntity entity, CancellationToken ct = default);

    Task DeleteAsync(TId id, CancellationToken ct = default);

    Task<bool> ExistsAsync(TId id, CancellationToken ct = default);
}
