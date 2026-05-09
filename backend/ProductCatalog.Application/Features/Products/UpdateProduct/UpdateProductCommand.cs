using MediatR;
using ProductCatalog.Application.Features.Products.GetProductById;

namespace ProductCatalog.Application.Features.Products.UpdateProduct;

public record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string? Description,
    decimal Price,
    string Currency,
    Guid CategoryId
) : IRequest<ProductDto>;
