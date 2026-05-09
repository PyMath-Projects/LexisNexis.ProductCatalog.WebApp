using MediatR;

namespace ProductCatalog.Application.Features.Products.SearchProducts;

public record SearchProductsQuery(string Query) : IRequest<SearchProductsResult>;
