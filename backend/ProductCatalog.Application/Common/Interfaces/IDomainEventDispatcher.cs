using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<AggregateRoot<Guid>> aggregates, CancellationToken ct = default);
}
