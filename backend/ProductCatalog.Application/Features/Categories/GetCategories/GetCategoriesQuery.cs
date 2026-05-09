using MediatR;

namespace ProductCatalog.Application.Features.Categories.GetCategories;

public record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;
