using MediatR;
using ProductCatalog.Domain.Products;

namespace ProductCatalog.Application.Features.Products.GetProducts;

public sealed class GetProductsHandler(IProductRepository productRepo)
    : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductSummaryDto>>
{
    /// <summary>Returns all products, optionally filtered by category. Results are sorted by price then name.</summary>
    public async Task<IReadOnlyList<ProductSummaryDto>> Handle(GetProductsQuery query, CancellationToken ct)
    {
        var products = query.CategoryId.HasValue
            ? await productRepo.GetByCategoryAsync(query.CategoryId.Value, ct)
            : await productRepo.GetAllAsync(ct);

        return products
            .OrderBy(p => p)
            .Select(p => new ProductSummaryDto(
                p.Id, p.Name.Value, p.Sku.Value,
                p.Price.Amount, p.Price.Currency,
                p.Quantity.Value, p.Status.ToString(), p.CategoryId))
            .ToList()
            .AsReadOnly();
    }
}
