using MediatR;
using ProductCatalog.Application.Common.Exceptions;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Features.Products.GetProductById;
using ProductCatalog.Domain.Categories;
using ProductCatalog.Domain.Products;

namespace ProductCatalog.Application.Features.Products.CreateProduct;

public sealed class CreateProductHandler(
    IProductRepository productRepo,
    ICategoryRepository categoryRepo,
    IDomainEventDispatcher dispatcher)
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    /// <summary>Creates a product after validating category existence and SKU uniqueness.</summary>
    public async Task<ProductDto> Handle(CreateProductCommand cmd, CancellationToken ct)
    {
        if (!await categoryRepo.ExistsAsync(cmd.CategoryId, ct))
            throw new NotFoundException(nameof(Category), cmd.CategoryId);

        if (await productRepo.SkuExistsAsync(cmd.Sku, ct: ct))
            throw new ValidationException($"SKU '{cmd.Sku}' is already in use.");

        var product = Product.Create(
            ProductName.Create(cmd.Name),
            cmd.Description,
            Sku.Create(cmd.Sku),
            Money.Of(cmd.Price, cmd.Currency),
            cmd.InitialQuantity,
            cmd.CategoryId);

        await productRepo.AddAsync(product, ct);
        await dispatcher.DispatchAsync([product], ct);

        return product.ToDto();
    }
}
