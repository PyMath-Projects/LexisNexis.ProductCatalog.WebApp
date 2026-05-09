using FluentAssertions;
using Moq;
using ProductCatalog.Application.Common.Exceptions;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Features.Products.UpdateProduct;
using ProductCatalog.Domain.Categories;
using ProductCatalog.Domain.Products;

namespace ProductCatalog.Application.Tests.Features.Products;

public sealed class UpdateProductHandlerTests
{
    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly Mock<ICategoryRepository> _categoryRepo = new();
    private readonly Mock<IDomainEventDispatcher> _dispatcher = new();

    private UpdateProductHandler CreateHandler() =>
        new(_productRepo.Object, _categoryRepo.Object, _dispatcher.Object);

    private static Product MakeProduct(Guid categoryId) =>
        Product.Create(ProductName.Create("Old Name"), null, Sku.Create("WI-000002"), Money.Of(5m, "USD"), 5, categoryId);

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUpdatedDto()
    {
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var product = MakeProduct(categoryId);
        _productRepo.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _categoryRepo.Setup(r => r.ExistsAsync(categoryId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var cmd = new UpdateProductCommand(productId, "New Name", "desc", 19.99m, "USD", categoryId);
        var result = await CreateHandler().Handle(cmd, CancellationToken.None);

        result.Name.Should().Be("New Name");
        result.Price.Should().Be(19.99m);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ThrowsNotFoundException()
    {
        var productId = Guid.NewGuid();
        _productRepo.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var cmd = new UpdateProductCommand(productId, "X", null, 1m, "USD", Guid.NewGuid());
        var act = () => CreateHandler().Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ThrowsNotFoundException()
    {
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var newCategoryId = Guid.NewGuid();
        var product = MakeProduct(categoryId);
        _productRepo.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _categoryRepo.Setup(r => r.ExistsAsync(newCategoryId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var cmd = new UpdateProductCommand(productId, "Name", null, 1m, "USD", newCategoryId);
        var act = () => CreateHandler().Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"*{newCategoryId}*");
    }
}
