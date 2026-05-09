using MediatR;
using ProductCatalog.Domain.Categories;

namespace ProductCatalog.Application.Features.Categories.GetCategories;

public sealed class GetCategoriesHandler(ICategoryRepository categoryRepo)
    : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    /// <summary>Returns all categories ordered by name.</summary>
    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery query, CancellationToken ct)
    {
        var categories = await categoryRepo.GetAllAsync(ct);

        return categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.ParentCategoryId, c.CreatedAt))
            .ToList()
            .AsReadOnly();
    }
}
