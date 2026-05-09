using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Domain.Categories;

public interface ICategoryRepository : IRepository<Category, Guid>
{
    Task<bool> HasChildCategoriesAsync(Guid categoryId, CancellationToken ct = default);
}
