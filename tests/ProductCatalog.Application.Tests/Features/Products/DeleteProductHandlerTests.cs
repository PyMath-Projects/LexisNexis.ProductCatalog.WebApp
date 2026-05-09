using FluentAssertions;
using Moq;
using ProductCatalog.Application.Common.Exceptions;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Features.Products.DeleteProduct;
using ProductCatalog.Domain.Products;

namespace ProductCatalog.Application.Tests.Features.Products;

public sealed class DeleteProductHandlerTests
{
    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly Mock<IDomainEventDispatcher> _dispatcher = new();

    private DeleteProductHandler CreateHandler() =>
        new(_productRepo.Object, _dispatcher.Object);

    private static Product MakeProduct() =>
        Product.Create(ProductName.Create("Widget"), null, Sku.Create("WI-000003"), Money.Of(5m, "USD"), 5, Guid.NewGuid());

    [Fact]
    public async Task Handle_ExistingProduct_DeletesAndDispatches()
    {
        var productId = Guid.NewGuid();
        var product = MakeProduct();
        _productRepo.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        await CreateHandler().Handle(new DeleteProductCommand(productId), CancellationToken.None);

        _productRepo.Verify(r => r.DeleteAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        _dispatcher.Verify(d => d.DispatchAsync(It.IsAny<IEnumerable<Domain.Shared.AggregateRoot<Guid>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ThrowsNotFoundException()
    {
        var productId = Guid.NewGuid();
        _productRepo.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var act = () => CreateHandler().Handle(new DeleteProductCommand(productId), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
