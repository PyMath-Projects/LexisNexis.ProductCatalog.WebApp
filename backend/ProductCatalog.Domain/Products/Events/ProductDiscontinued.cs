using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Domain.Products.Events;

public record ProductDiscontinued(Guid ProductId, DateTimeOffset OccurredAt) : IDomainEvent;
