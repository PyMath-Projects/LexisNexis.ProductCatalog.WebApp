using ProductCatalog.Application.Features.Products.GetProducts;

namespace ProductCatalog.Application.Features.Products.SearchProducts;

public record SearchProductsResult(IReadOnlyList<ProductSummaryDto> Products, bool CacheHit);
