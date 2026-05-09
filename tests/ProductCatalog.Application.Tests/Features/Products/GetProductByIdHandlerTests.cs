using FluentAssertions;
using Moq;
using ProductCatalog.Application.Common.Exceptions;
using ProductCatalog.Application.Features.Products.GetProductById;
using ProductCatalog.Domain.Products;

namespace ProductCatalog.Application.Tests.Features.Products;

public sealed class GetProductByIdHandlerTests
{
    private readonly Mock<IProductRepository> _productRepo = new();

    private GetProductByIdHandler CreateHandler() => new(_productRepo.Object);

    private static Product MakeProduct(Guid categoryId) =>
        Product.Create(ProductName.Create("Widget"), "desc", Sku.Create("WI-000006"), Money.Of(9.99m, "USD"), 5, categoryId);

    [Fact]
    public async Task Handle_ExistingProduct_ReturnsDto()
    {
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var product = MakeProduct(categoryId);
        _productRepo.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        var result = await CreateHandler().Handle(new GetProductByIdQuery(productId), CancellationToken.None);

        result.Name.Should().Be("Widget");
        result.Sku.Should().Be("WI-000006");
        result.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public async Task Handle_MissingProduct_ThrowsNotFoundException()
    {
        var productId = Guid.NewGuid();
        _productRepo.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var act = () => CreateHandler().Handle(new GetProductByIdQuery(productId), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"*{productId}*");
    }
}
