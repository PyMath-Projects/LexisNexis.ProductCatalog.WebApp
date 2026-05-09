using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Domain.Products;

public interface IProductRepository : IRepository<Product, Guid>
{
    Task<bool> SkuExistsAsync(string skuValue, Guid? excludeProductId = null, CancellationToken ct = default);

    Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default);
}
