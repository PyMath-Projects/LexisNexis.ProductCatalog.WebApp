using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Categories;

namespace ProductCatalog.Infrastructure.Persistence;

public sealed class CategoryRepository(ProductCatalogDbContext context)
    : RepositoryBase<Category, Guid>(context), ICategoryRepository
{
    public async Task<bool> HasChildCategoriesAsync(Guid categoryId, CancellationToken ct = default) =>
        await Context.Categories
            .AnyAsync(c => c.ParentCategoryId == categoryId, ct);
}
