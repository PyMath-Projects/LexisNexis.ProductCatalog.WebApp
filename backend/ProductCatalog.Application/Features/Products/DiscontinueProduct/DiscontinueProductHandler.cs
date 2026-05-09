using MediatR;
using ProductCatalog.Application.Common.Exceptions;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Domain.Products;

namespace ProductCatalog.Application.Features.Products.DiscontinueProduct;

public sealed class DiscontinueProductHandler(
    IProductRepository productRepo,
    IDomainEventDispatcher dispatcher)
    : IRequestHandler<DiscontinueProductCommand>
{
    /// <summary>Marks a product as discontinued. Throws when the product is already discontinued.</summary>
    public async Task Handle(DiscontinueProductCommand cmd, CancellationToken ct)
    {
        var product = await productRepo.GetByIdAsync(cmd.ProductId, ct)
            ?? throw new NotFoundException(nameof(Product), cmd.ProductId);

        product.Discontinue();

        await productRepo.UpdateAsync(product, ct);
        await dispatcher.DispatchAsync([product], ct);
    }
}
