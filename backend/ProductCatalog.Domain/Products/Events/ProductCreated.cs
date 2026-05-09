using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Domain.Products.Events;

public record ProductCreated(Guid ProductId, string SkuValue, DateTimeOffset OccurredAt) : IDomainEvent;
