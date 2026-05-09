using FluentAssertions;
using Moq;
using ProductCatalog.Application.Features.Products.GetProducts;
using ProductCatalog.Domain.Products;

namespace ProductCatalog.Application.Tests.Features.Products;

public sealed class GetProductsHandlerTests
{
    private readonly Mock<IProductRepository> _productRepo = new();

    private GetProductsHandler CreateHandler() => new(_productRepo.Object);

    private static Product MakeProduct(string name, string sku, decimal price, Guid categoryId) =>
        Product.Create(ProductName.Create(name), null, Sku.Create(sku), Money.Of(price, "USD"), 5, categoryId);

    [Fact]
    public async Task Handle_NoCategoryFilter_ReturnsAllProductsSorted()
    {
        var catId = Guid.NewGuid();
        var products = new[]
        {
            MakeProduct("Zebra",  "ZE-000001", 50m, catId),
            MakeProduct("Alpha",  "AL-000001", 10m, catId),
            MakeProduct("Middle", "MI-000001", 10m, catId),
        };
        _productRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(products);

        var result = await CreateHandler().Handle(new GetProductsQuery(), CancellationToken.None);

        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Alpha");
        result[1].Name.Should().Be("Middle");
        result[2].Name.Should().Be("Zebra");
    }

    [Fact]
    public async Task Handle_WithCategoryFilter_QueriesByCategory()
    {
        var categoryId = Guid.NewGuid();
        _productRepo.Setup(r => r.GetByCategoryAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Product>());

        var result = await CreateHandler().Handle(new GetProductsQuery(categoryId), CancellationToken.None);

        result.Should().BeEmpty();
        _productRepo.Verify(r => r.GetByCategoryAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
