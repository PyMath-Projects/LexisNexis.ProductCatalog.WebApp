using MediatR;
using ProductCatalog.Application.Common.Events;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Domain.Products.Events;

namespace ProductCatalog.Application.Handlers;

public sealed class SearchCacheInvalidator(ISearchCache cache)
    : INotificationHandler<DomainEventNotification<ProductCreated>>,
      INotificationHandler<DomainEventNotification<ProductDeleted>>,
      INotificationHandler<DomainEventNotification<ProductDiscontinued>>,
      INotificationHandler<DomainEventNotification<ProductReactivated>>
{
    public Task Handle(DomainEventNotification<ProductCreated> n, CancellationToken ct)
    {
        cache.Invalidate("products:");
        return Task.CompletedTask;
    }

    public Task Handle(DomainEventNotification<ProductDeleted> n, CancellationToken ct)
    {
        cache.Invalidate("products:");
        return Task.CompletedTask;
    }

    public Task Handle(DomainEventNotification<ProductDiscontinued> n, CancellationToken ct)
    {
        cache.Invalidate("products:");
        return Task.CompletedTask;
    }

    public Task Handle(DomainEventNotification<ProductReactivated> n, CancellationToken ct)
    {
        cache.Invalidate("products:");
        return Task.CompletedTask;
    }
}
