using FluentAssertions;
using Moq;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Features.Products.GetProducts;
using ProductCatalog.Application.Features.Products.SearchProducts;
using ProductCatalog.Domain.Products;

namespace ProductCatalog.Application.Tests.Features.Products;

public sealed class SearchProductsHandlerTests
{
    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly Mock<ISearchCache> _cache = new();

    private SearchProductsHandler CreateHandler() => new(_productRepo.Object, _cache.Object);

    private static Product MakeProduct(string name, string sku) =>
        Product.Create(ProductName.Create(name), null, Sku.Create(sku), Money.Of(5m, "USD"), 5, Guid.NewGuid());

    [Fact]
    public async Task Handle_QueryMatchesName_ReturnsMatchingProduct()
    {
        var products = new[] { MakeProduct("Laptop Stand", "LA-000001"), MakeProduct("Mouse Pad", "MO-000001") };
        _productRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(products);
        _cache.Setup(c => c.TryGet<object>(It.IsAny<string>(), out It.Ref<object?>.IsAny)).Returns(false);

        var result = await CreateHandler().Handle(new SearchProductsQuery("Laptop"), CancellationToken.None);

        result.Products.Should().HaveCount(1);
        result.Products[0].Name.Should().Be("Laptop Stand");
        result.CacheHit.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_CachedResult_ReturnsCacheWithoutHittingRepo()
    {
        var cached = Array.Empty<ProductSummaryDto>().ToList().AsReadOnly() as IReadOnlyList<ProductSummaryDto>;
        _cache.Setup(c => c.TryGet<IReadOnlyList<ProductSummaryDto>>(It.IsAny<string>(), out cached)).Returns(true);

        var result = await CreateHandler().Handle(new SearchProductsQuery("anything"), CancellationToken.None);

        _productRepo.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Never);
        result.CacheHit.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_EmptyQuery_ReturnsAllProducts()
    {
        var products = new[] { MakeProduct("Widget A", "WA-000001"), MakeProduct("Widget B", "WB-000001") };
        _productRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(products);
        _cache.Setup(c => c.TryGet<object>(It.IsAny<string>(), out It.Ref<object?>.IsAny)).Returns(false);

        var result = await CreateHandler().Handle(new SearchProductsQuery("   "), CancellationToken.None);

        result.Products.Should().HaveCount(2);
        result.CacheHit.Should().BeFalse();
    }
}
