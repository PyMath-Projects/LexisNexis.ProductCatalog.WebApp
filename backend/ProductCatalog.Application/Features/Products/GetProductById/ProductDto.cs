namespace ProductCatalog.Application.Features.Products.GetProductById;

public record ProductDto(
    Guid Id,
    string Name,
    string? Description,
    string Sku,
    decimal Price,
    string Currency,
    int Quantity,
    Guid CategoryId,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
