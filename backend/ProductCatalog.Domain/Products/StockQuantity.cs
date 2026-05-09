using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Domain.Products;

public sealed class StockQuantity : ValueObject
{
    public int Value { get; }

    private StockQuantity(int value) => Value = value;

    public static StockQuantity Of(int value)
    {
        Guard.AgainstNegative(value, nameof(value));
        return new StockQuantity(value);
    }

    public StockQuantity Adjust(int delta)
    {
        int result = Value + delta;
        if (result < 0)
        {
            throw new DomainException($"Insufficient stock. Available: {Value}, requested change: {delta}.");
        }

        return new StockQuantity(result);
    }

    public bool IsEmpty => Value == 0;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
