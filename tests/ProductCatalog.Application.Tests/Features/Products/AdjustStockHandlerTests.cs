using FluentAssertions;
using Moq;
using ProductCatalog.Application.Common.Exceptions;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Features.Products.AdjustStock;
using ProductCatalog.Domain.Products;
using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Application.Tests.Features.Products;

public sealed class AdjustStockHandlerTests
{
    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly Mock<IDomainEventDispatcher> _dispatcher = new();

    private AdjustStockHandler CreateHandler() =>
        new(_productRepo.Object, _dispatcher.Object);

    private static Product MakeProduct(int initialQty = 10) =>
        Product.Create(ProductName.Create("Widget"), null, Sku.Create("WI-000004"), Money.Of(5m, "USD"), initialQty, Guid.NewGuid());

    [Fact]
    public async Task Handle_ValidDelta_UpdatesAndDispatches()
    {
        var productId = Guid.NewGuid();
        var product = MakeProduct(10);
        _productRepo.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        await CreateHandler().Handle(new AdjustStockCommand(productId, -3), CancellationToken.None);

        product.Quantity.Value.Should().Be(7);
        _productRepo.Verify(r => r.UpdateAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        _dispatcher.Verify(d => d.DispatchAsync(It.IsAny<IEnumerable<AggregateRoot<Guid>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ThrowsNotFoundException()
    {
        var productId = Guid.NewGuid();
        _productRepo.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var act = () => CreateHandler().Handle(new AdjustStockCommand(productId, 5), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_NegativeDeltaExceedsStock_ThrowsDomainException()
    {
        var productId = Guid.NewGuid();
        var product = MakeProduct(2);
        _productRepo.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        var act = () => CreateHandler().Handle(new AdjustStockCommand(productId, -5), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }
}
