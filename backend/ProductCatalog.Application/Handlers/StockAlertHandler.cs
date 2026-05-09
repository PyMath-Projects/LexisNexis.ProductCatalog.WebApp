using MediatR;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Common.Events;
using ProductCatalog.Domain.Products.Events;

namespace ProductCatalog.Application.Handlers;

public sealed class StockAlertHandler(ILogger<StockAlertHandler> logger)
    : INotificationHandler<DomainEventNotification<StockLevelChanged>>
{
    public Task Handle(DomainEventNotification<StockLevelChanged> n, CancellationToken ct)
    {
        var e = n.DomainEvent;
        if (e.Current == 0)
            logger.LogWarning("Product {ProductId} is now out of stock.", e.ProductId);
        else if (e.Current <= 5)
            logger.LogInformation("Product {ProductId} low stock: {Qty} remaining.", e.ProductId, e.Current);
        return Task.CompletedTask;
    }
}
