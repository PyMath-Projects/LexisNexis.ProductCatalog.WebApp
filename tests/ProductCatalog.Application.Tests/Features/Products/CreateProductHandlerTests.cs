using FluentAssertions;
using Moq;
using ProductCatalog.Application.Common.Exceptions;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Features.Products.CreateProduct;
using ProductCatalog.Domain.Categories;
using ProductCatalog.Domain.Products;

namespace ProductCatalog.Application.Tests.Features.Products;

public sealed class CreateProductHandlerTests
{
    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly Mock<ICategoryRepository> _categoryRepo = new();
    private readonly Mock<IDomainEventDispatcher> _dispatcher = new();

    private CreateProductHandler CreateHandler() =>
        new(_productRepo.Object, _categoryRepo.Object, _dispatcher.Object);

    private static CreateProductCommand ValidCommand(Guid categoryId) => new(
        "Test Widget", "A widget", "WI-000001", 9.99m, "USD", 10, categoryId);

    [Fact]
    public async Task Handle_ValidCommand_ReturnsProductDto()
    {
        var categoryId = Guid.NewGuid();
        _categoryRepo.Setup(r => r.ExistsAsync(categoryId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _productRepo.Setup(r => r.SkuExistsAsync("WI-000001", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await CreateHandler().Handle(ValidCommand(categoryId), CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Test Widget");
        result.Sku.Should().Be("WI-000001");
        result.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ThrowsNotFoundException()
    {
        var categoryId = Guid.NewGuid();
        _categoryRepo.Setup(r => r.ExistsAsync(categoryId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var act = () => CreateHandler().Handle(ValidCommand(categoryId), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"*{categoryId}*");
    }

    [Fact]
    public async Task Handle_DuplicateSku_ThrowsValidationException()
    {
        var categoryId = Guid.NewGuid();
        _categoryRepo.Setup(r => r.ExistsAsync(categoryId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _productRepo.Setup(r => r.SkuExistsAsync("WI-000001", null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var act = () => CreateHandler().Handle(ValidCommand(categoryId), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>().WithMessage("*WI-000001*");
    }

    [Fact]
    public async Task Handle_ValidCommand_AddsProductAndDispatchesEvents()
    {
        var categoryId = Guid.NewGuid();
        _categoryRepo.Setup(r => r.ExistsAsync(categoryId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _productRepo.Setup(r => r.SkuExistsAsync("WI-000001", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await CreateHandler().Handle(ValidCommand(categoryId), CancellationToken.None);

        _productRepo.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _dispatcher.Verify(d => d.DispatchAsync(It.IsAny<IEnumerable<Domain.Shared.AggregateRoot<Guid>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
