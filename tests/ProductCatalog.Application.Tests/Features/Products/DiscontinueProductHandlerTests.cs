using FluentAssertions;
using Moq;
using ProductCatalog.Application.Common.Exceptions;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Features.Products.DiscontinueProduct;
using ProductCatalog.Domain.Products;
using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Application.Tests.Features.Products;

public sealed class DiscontinueProductHandlerTests
{
    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly Mock<IDomainEventDispatcher> _dispatcher = new();

    private DiscontinueProductHandler CreateHandler() =>
        new(_productRepo.Object, _dispatcher.Object);

    private static Product MakeProduct() =>
        Product.Create(ProductName.Create("Widget"), null, Sku.Create("WI-000005"), Money.Of(5m, "USD"), 5, Guid.NewGuid());

    [Fact]
    public async Task Handle_ActiveProduct_DiscontinuesAndDispatches()
    {
        var productId = Guid.NewGuid();
        var product = MakeProduct();
        _productRepo.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        await CreateHandler().Handle(new DiscontinueProductCommand(productId), CancellationToken.None);

        product.Status.Should().Be(ProductStatus.Discontinued);
        _dispatcher.Verify(d => d.DispatchAsync(It.IsAny<IEnumerable<AggregateRoot<Guid>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ThrowsNotFoundException()
    {
        var productId = Guid.NewGuid();
        _productRepo.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var act = () => CreateHandler().Handle(new DiscontinueProductCommand(productId), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_AlreadyDiscontinued_ThrowsDomainException()
    {
        var productId = Guid.NewGuid();
        var product = MakeProduct();
        product.Discontinue();
        _productRepo.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        var act = () => CreateHandler().Handle(new DiscontinueProductCommand(productId), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }
}
