using MediatR;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Features.Products.GetProducts;
using ProductCatalog.Domain.Products;

namespace ProductCatalog.Application.Features.Products.SearchProducts;

public sealed class SearchProductsHandler(IProductRepository productRepo, ISearchCache cache)
    : IRequestHandler<SearchProductsQuery, IReadOnlyList<ProductSummaryDto>>
{
    private static readonly ProductSearchEngine _engine = new();

    /// <summary>Returns products matching the query using cache-then-search. Cache key is prefixed with "products:".</summary>
    public async Task<IReadOnlyList<ProductSummaryDto>> Handle(SearchProductsQuery query, CancellationToken ct)
    {
        var cacheKey = $"products:{query.Query.Trim().ToUpperInvariant()}";

        if (cache.TryGet<IReadOnlyList<ProductSummaryDto>>(cacheKey, out var cached) && cached is not null)
            return cached;

        var all = (await productRepo.GetAllAsync(ct)).ToList();

        var fields = new (Func<Product, string?> Field, int Weight)[]
        {
            (p => p.Name.Value,       3),
            (p => p.Sku.Value,        2),
            (p => p.Description,      1),
        };

        var results = _engine.Search(all, query.Query, fields)
            .Select(p => new ProductSummaryDto(
                p.Id, p.Name.Value, p.Sku.Value,
                p.Price.Amount, p.Price.Currency,
                p.Quantity.Value, p.Status.ToString(), p.CategoryId))
            .ToList()
            .AsReadOnly();

        cache.Set(cacheKey, results);
        return results;
    }
}
