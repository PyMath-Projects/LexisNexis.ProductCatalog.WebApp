namespace ProductCatalog.Domain.Shared;

public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}
