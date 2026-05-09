using MediatR;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Infrastructure.Persistence;

public sealed class DomainEventDispatcher(IMediator mediator) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IEnumerable<AggregateRoot<Guid>> aggregates, CancellationToken ct = default)
    {
        foreach (var aggregate in aggregates)
        {
            foreach (var domainEvent in aggregate.GetDomainEvents())
            {
                var notificationType = typeof(Application.Common.Events.DomainEventNotification<>)
                    .MakeGenericType(domainEvent.GetType());

                var notification = Activator.CreateInstance(notificationType, domainEvent)
                    as INotification
                    ?? throw new InvalidOperationException(
                        $"Could not create notification for {domainEvent.GetType().Name}");

                await mediator.Publish(notification, ct);
            }

            aggregate.ClearDomainEvents();
        }
    }
}
