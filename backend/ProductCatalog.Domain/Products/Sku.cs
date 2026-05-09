using System.Text.RegularExpressions;
using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Domain.Products;

public sealed partial class Sku : ValueObject
{
    public string Value { get; }

    private Sku(string value) => Value = value;

    public static Sku Create(string value)
    {
        string upper = Guard.AgainstNullOrWhiteSpace(value, nameof(value)).ToUpperInvariant().Trim();
        if (!SkuFormat().IsMatch(upper))
        {
            throw new DomainException($"SKU '{value}' is not valid. Expected format: XX-000000 (e.g. EL-004521).");
        }

        return new Sku(upper);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    [GeneratedRegex("^[A-Z]{2}-\\d{6}$")]
    private static partial Regex SkuFormat();
}
