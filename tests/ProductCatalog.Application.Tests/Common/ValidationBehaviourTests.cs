using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using ProductCatalog.Application.Common.Behaviours;
using ProductCatalog.Application.Common.Exceptions;
using ProductCatalog.Application.Features.Products.CreateProduct;
using ProductCatalog.Application.Features.Products.GetProductById;

namespace ProductCatalog.Application.Tests.Common;

public sealed class ValidationBehaviourTests
{
    private ValidationBehaviour<CreateProductCommand, ProductDto> CreateBehaviour() =>
        new(NullLogger<ValidationBehaviour<CreateProductCommand, ProductDto>>.Instance);

    [Fact]
    public async Task Handle_ValidCommand_CallsNext()
    {
        var cmd = new CreateProductCommand("Name", null, "NA-000001", 1m, "USD", 0, Guid.NewGuid());
        var called = false;

        await CreateBehaviour().Handle(cmd, ct =>
        {
            called = true;
            return Task.FromResult(default(ProductDto)!);
        }, CancellationToken.None);

        called.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "WI-000001", 1.0, 0, "Name is required")]
    [InlineData("Name", "WI-000001", 0.0, 0, "Price must be positive")]
    [InlineData("Name", "", 1.0, 0, "SKU is required")]
    [InlineData("Name", "WI-000001", 1.0, -1, "Quantity cannot be negative")]
    public async Task Handle_InvalidCommand_ThrowsValidationException(
        string name, string sku, double price, int qty, string expectedMessage)
    {
        var cmd = new CreateProductCommand(name, null, sku, (decimal)price, "USD", qty, Guid.NewGuid());

        var act = () => CreateBehaviour().Handle(cmd, ct => Task.FromResult(default(ProductDto)!), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>().WithMessage(expectedMessage);
    }

    [Fact]
    public async Task Handle_EmptyCategoryId_ThrowsValidationException()
    {
        var cmd = new CreateProductCommand("Name", null, "WI-000001", 1m, "USD", 0, Guid.Empty);

        var act = () => CreateBehaviour().Handle(cmd, ct => Task.FromResult(default(ProductDto)!), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>().WithMessage("CategoryId is required");
    }
}
