using ProductCatalog.Domain.Products;

namespace ProductCatalog.Application.Features.Products.GetProductById;

internal static class ProductMappings
{
    internal static ProductDto ToDto(this Product p) => new(
        p.Id, p.Name.Value, p.Description, p.Sku.Value,
        p.Price.Amount, p.Price.Currency,
        p.Quantity.Value, p.CategoryId, p.Status.ToString(),
        p.CreatedAt, p.UpdatedAt);
}
