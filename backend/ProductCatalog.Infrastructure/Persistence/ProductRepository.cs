using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Products;

namespace ProductCatalog.Infrastructure.Persistence;

public sealed class ProductRepository(ProductCatalogDbContext context)
    : RepositoryBase<Product, Guid>(context), IProductRepository
{
    public async Task<bool> SkuExistsAsync(
        string skuValue,
        Guid? excludeProductId = null,
        CancellationToken ct = default)
    {
        var query = Context.Products
            .Where(p => p.Sku.Value == skuValue);

        if (excludeProductId.HasValue)
            query = query.Where(p => p.Id != excludeProductId.Value);

        return await query.AnyAsync(ct);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(
        Guid categoryId,
        CancellationToken ct = default) =>
        await Context.Products
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync(ct);
}
