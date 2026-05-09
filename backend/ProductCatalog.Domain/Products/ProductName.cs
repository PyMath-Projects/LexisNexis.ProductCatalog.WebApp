using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Domain.Products;

public sealed class ProductName : ValueObject
{
    public string Value { get; }

    private ProductName(string value) => Value = value;

    public static ProductName Create(string value)
    {
        string cleaned = Guard.AgainstNullOrWhiteSpace(value, nameof(value)).Trim();
        if (cleaned.Length > 200)
        {
            throw new DomainException("Product name cannot exceed 200 characters.");
        }

        return new ProductName(cleaned);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value.ToUpperInvariant();
    }

    public override string ToString() => Value;
}
