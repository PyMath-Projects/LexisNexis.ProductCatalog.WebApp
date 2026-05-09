using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Domain.Products.Events;

public record StockLevelChanged(Guid ProductId, int Previous, int Current, DateTimeOffset OccurredAt) : IDomainEvent;
