namespace ProductCatalog.Domain.Categories.Events;

public record CategoryCreated(Guid CategoryId, Guid? ParentId, DateTimeOffset OccurredAt)
    : Shared.IDomainEvent;
