using MediatR;
using ProductCatalog.Application.Common.Exceptions;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Features.Products.GetProductById;
using ProductCatalog.Domain.Categories;
using ProductCatalog.Domain.Products;

namespace ProductCatalog.Application.Features.Products.UpdateProduct;

public sealed class UpdateProductHandler(
    IProductRepository productRepo,
    ICategoryRepository categoryRepo,
    IDomainEventDispatcher dispatcher)
    : IRequestHandler<UpdateProductCommand, ProductDto>
{
    /// <summary>Updates product details after verifying the product and target category exist.</summary>
    public async Task<ProductDto> Handle(UpdateProductCommand cmd, CancellationToken ct)
    {
        var product = await productRepo.GetByIdAsync(cmd.ProductId, ct)
            ?? throw new NotFoundException(nameof(Product), cmd.ProductId);

        if (!await categoryRepo.ExistsAsync(cmd.CategoryId, ct))
            throw new NotFoundException(nameof(Category), cmd.CategoryId);

        product.UpdateDetails(
            ProductName.Create(cmd.Name),
            cmd.Description,
            Money.Of(cmd.Price, cmd.Currency),
            cmd.CategoryId);

        await productRepo.UpdateAsync(product, ct);
        await dispatcher.DispatchAsync([product], ct);

        return product.ToDto();
    }
}
