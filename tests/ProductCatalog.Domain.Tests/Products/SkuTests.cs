using FluentAssertions;
using ProductCatalog.Domain.Products;
using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Domain.Tests.Products;

public sealed class SkuTests
{
    [Fact]
    public void Create_WhenValueIsInvalid_ThrowsDomainException()
    {
        Action act = () => Sku.Create("INVALID");

        act.Should().Throw<DomainException>()
            .WithMessage("SKU 'INVALID' is not valid*");
    }

    [Fact]
    public void Create_WhenValueMatchesExpectedFormat_Succeeds()
    {
        Sku sku = Sku.Create("EL-004521");

        sku.Value.Should().Be("EL-004521");
    }

    [Fact]
    public void Equals_WhenValuesDifferOnlyByCase_ReturnsTrue()
    {
        Sku first = Sku.Create("EL-004521");
        Sku second = Sku.Create("el-004521");

        first.Should().Be(second);
        (first == second).Should().BeTrue();
    }
}
