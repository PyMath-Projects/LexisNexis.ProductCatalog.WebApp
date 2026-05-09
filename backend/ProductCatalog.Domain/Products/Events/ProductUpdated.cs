using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Domain.Products.Events;

public record ProductUpdated(Guid ProductId, DateTimeOffset OccurredAt) : IDomainEvent;
