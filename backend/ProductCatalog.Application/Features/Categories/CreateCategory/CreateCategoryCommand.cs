using MediatR;
using ProductCatalog.Application.Features.Categories.GetCategories;

namespace ProductCatalog.Application.Features.Categories.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string? Description,
    Guid? ParentCategoryId
) : IRequest<CategoryDto>;
