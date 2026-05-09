namespace ProductCatalog.Domain.Shared;

public static class Guard
{
    public static string AgainstNullOrWhiteSpace(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{paramName} cannot be null or whitespace.");
        }

        return value;
    }

    public static decimal AgainstNegativeOrZero(decimal value, string paramName)
    {
        if (value <= 0)
        {
            throw new DomainException($"{paramName} must be greater than zero.");
        }

        return value;
    }

    public static int AgainstNegative(int value, string paramName)
    {
        if (value < 0)
        {
            throw new DomainException($"{paramName} cannot be negative.");
        }

        return value;
    }
}
