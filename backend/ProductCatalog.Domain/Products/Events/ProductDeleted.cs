namespace ProductCatalog.Domain.Products.Events;

public record ProductDeleted(Guid ProductId, DateTimeOffset OccurredAt)
    : Shared.IDomainEvent;
