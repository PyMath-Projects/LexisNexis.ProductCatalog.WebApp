namespace ProductCatalog.Application.Features.Products.GetProducts;

public record ProductSummaryDto(
    Guid Id,
    string Name,
    string Sku,
    decimal Price,
    string Currency,
    int Quantity,
    string Status,
    Guid CategoryId);
