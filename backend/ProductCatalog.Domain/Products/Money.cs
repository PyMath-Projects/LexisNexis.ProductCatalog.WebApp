using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Domain.Products;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }

    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Of(decimal amount, string currency = "USD")
    {
        Guard.AgainstNegativeOrZero(amount, nameof(amount));
        Guard.AgainstNullOrWhiteSpace(currency, nameof(currency));

        return new Money(Math.Round(amount, 2), currency.ToUpperInvariant());
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new DomainException($"Cannot add money with different currencies: {Currency} and {other.Currency}.");
        }

        return new Money(Amount + other.Amount, Currency);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Currency} {Amount:F2}";
}
