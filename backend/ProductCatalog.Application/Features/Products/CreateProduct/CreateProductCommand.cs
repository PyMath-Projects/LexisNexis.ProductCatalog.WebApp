using MediatR;
using ProductCatalog.Application.Features.Products.GetProductById;

namespace ProductCatalog.Application.Features.Products.CreateProduct;

public record CreateProductCommand(
    string Name,
    string? Description,
    string Sku,
    decimal Price,
    string Currency,
    int InitialQuantity,
    Guid CategoryId
) : IRequest<ProductDto>;
