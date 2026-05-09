using MediatR;
using ProductCatalog.Application.Common.Exceptions;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Domain.Products;

namespace ProductCatalog.Application.Features.Products.AdjustStock;

public sealed class AdjustStockHandler(
    IProductRepository productRepo,
    IDomainEventDispatcher dispatcher)
    : IRequestHandler<AdjustStockCommand>
{
    /// <summary>Adjusts stock by the given delta. Negative delta represents a sale; positive represents a restock.</summary>
    public async Task Handle(AdjustStockCommand cmd, CancellationToken ct)
    {
        var product = await productRepo.GetByIdAsync(cmd.ProductId, ct)
            ?? throw new NotFoundException(nameof(Product), cmd.ProductId);

        product.AdjustStock(cmd.Delta);

        await productRepo.UpdateAsync(product, ct);
        await dispatcher.DispatchAsync([product], ct);
    }
}
