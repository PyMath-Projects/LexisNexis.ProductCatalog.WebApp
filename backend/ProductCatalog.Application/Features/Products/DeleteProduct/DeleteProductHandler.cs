using MediatR;
using ProductCatalog.Application.Common.Exceptions;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Domain.Products;

namespace ProductCatalog.Application.Features.Products.DeleteProduct;

public sealed class DeleteProductHandler(
    IProductRepository productRepo,
    IDomainEventDispatcher dispatcher)
    : IRequestHandler<DeleteProductCommand>
{
    /// <summary>Deletes a product and dispatches ProductDeleted so dependent handlers (e.g. cache invalidation) react.</summary>
    public async Task Handle(DeleteProductCommand cmd, CancellationToken ct)
    {
        var product = await productRepo.GetByIdAsync(cmd.ProductId, ct)
            ?? throw new NotFoundException(nameof(Product), cmd.ProductId);

        product.MarkAsDeleted();
        await productRepo.DeleteAsync(cmd.ProductId, ct);
        await dispatcher.DispatchAsync([product], ct);
    }
}
