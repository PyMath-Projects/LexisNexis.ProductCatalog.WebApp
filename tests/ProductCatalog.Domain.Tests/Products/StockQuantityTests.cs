using FluentAssertions;
using ProductCatalog.Domain.Products;
using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Domain.Tests.Products;

public sealed class StockQuantityTests
{
    [Fact]
    public void Of_WhenValueIsNegative_ThrowsDomainException()
    {
        Action act = () => StockQuantity.Of(-1);

        act.Should().Throw<DomainException>()
            .WithMessage("value cannot be negative.");
    }

    [Fact]
    public void Adjust_WhenResultWouldBeBelowZero_ThrowsDomainException()
    {
        StockQuantity quantity = StockQuantity.Of(3);

        Action act = () => quantity.Adjust(-4);

        act.Should().Throw<DomainException>()
            .WithMessage("Insufficient stock*");
    }

    [Fact]
    public void IsEmpty_WhenValueIsZero_ReturnsTrue()
    {
        StockQuantity quantity = StockQuantity.Of(0);

        quantity.IsEmpty.Should().BeTrue();
    }
}
