using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Domain.Products.Events;

public record ProductReactivated(Guid ProductId, DateTimeOffset OccurredAt) : IDomainEvent;
