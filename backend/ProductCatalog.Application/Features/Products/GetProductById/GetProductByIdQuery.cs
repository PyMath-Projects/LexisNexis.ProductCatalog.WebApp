using MediatR;

namespace ProductCatalog.Application.Features.Products.GetProductById;

public record GetProductByIdQuery(Guid ProductId) : IRequest<ProductDto>;
