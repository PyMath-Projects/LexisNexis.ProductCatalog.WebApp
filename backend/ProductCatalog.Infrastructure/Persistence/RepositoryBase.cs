using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Infrastructure.Persistence;

public abstract class RepositoryBase<TEntity, TId>(ProductCatalogDbContext context)
    : IRepository<TEntity, TId>
    where TEntity : AggregateRoot<TId>
    where TId : notnull
{
    protected readonly ProductCatalogDbContext Context = context;

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default) =>
        await Context.Set<TEntity>().FindAsync([id], ct);

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct = default) =>
        await Context.Set<TEntity>().ToListAsync(ct);

    public async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await Context.Set<TEntity>().AddAsync(entity, ct);
        await Context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        Context.Set<TEntity>().Update(entity);
        await Context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(TId id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity is not null)
        {
            Context.Set<TEntity>().Remove(entity);
            await Context.SaveChangesAsync(ct);
        }
    }

    public async Task<bool> ExistsAsync(TId id, CancellationToken ct = default) =>
        await GetByIdAsync(id, ct) is not null;
}
