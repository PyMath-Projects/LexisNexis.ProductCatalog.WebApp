using MediatR;

namespace ProductCatalog.Application.Features.Products.GetProducts;

public record GetProductsQuery(Guid? CategoryId = null) : IRequest<IReadOnlyList<ProductSummaryDto>>;
