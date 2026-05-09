using MediatR;

namespace ProductCatalog.Application.Features.Products.DeleteProduct;

public record DeleteProductCommand(Guid ProductId) : IRequest;
