using MediatR;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Common.Events;
using ProductCatalog.Domain.Categories.Events;

namespace ProductCatalog.Application.Handlers;

public sealed class OnCategoryCreated(ILogger<OnCategoryCreated> logger)
    : INotificationHandler<DomainEventNotification<CategoryCreated>>
{
    public Task Handle(DomainEventNotification<CategoryCreated> n, CancellationToken ct)
    {
        logger.LogInformation("Category {CategoryId} created.", n.DomainEvent.CategoryId);
        return Task.CompletedTask;
    }
}
