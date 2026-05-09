using MediatR;

namespace ProductCatalog.Application.Features.Categories.GetCategoryTree;

public record GetCategoryTreeQuery : IRequest<IReadOnlyList<CategoryTreeDto>>;
