using MediatR;

namespace ProductCatalog.Application.Features.Products.DiscontinueProduct;

public record DiscontinueProductCommand(Guid ProductId) : IRequest;
