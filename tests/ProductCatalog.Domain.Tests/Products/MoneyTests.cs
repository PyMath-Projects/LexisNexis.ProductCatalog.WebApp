using FluentAssertions;
using ProductCatalog.Domain.Products;
using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Domain.Tests.Products;

public sealed class MoneyTests
{
    [Fact]
    public void Of_WhenAmountIsNegative_ThrowsDomainException()
    {
        Action act = () => Money.Of(-1m);

        act.Should().Throw<DomainException>()
            .WithMessage("amount must be greater than zero.");
    }

    [Fact]
    public void Of_WhenAmountHasMoreThanTwoDecimals_RoundsAmount()
    {
        Money money = Money.Of(10.126m, "usd");

        money.Amount.Should().Be(10.13m);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Add_WhenCurrenciesDiffer_ThrowsDomainException()
    {
        Money usd = Money.Of(10m, "USD");
        Money eur = Money.Of(10m, "EUR");

        Action act = () => usd.Add(eur);

        act.Should().Throw<DomainException>()
            .WithMessage("Cannot add money with different currencies*");
    }
}
