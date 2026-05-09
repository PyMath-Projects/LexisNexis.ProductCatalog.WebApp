namespace ProductCatalog.Domain.Shared;

public sealed class DomainException(string message) : Exception(message);
