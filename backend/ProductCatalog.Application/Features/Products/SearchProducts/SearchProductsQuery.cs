using MediatR;
using ProductCatalog.Application.Features.Products.GetProducts;

namespace ProductCatalog.Application.Features.Products.SearchProducts;

public record SearchProductsQuery(string Query) : IRequest<IReadOnlyList<ProductSummaryDto>>;
