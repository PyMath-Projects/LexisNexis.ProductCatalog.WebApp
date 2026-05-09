using MediatR;
using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Application.Common.Events;

/// <summary>MediatR bridge: wraps a plain IDomainEvent into an INotification so MediatR can publish it.</summary>
public record DomainEventNotification<T>(T DomainEvent) : INotification
    where T : IDomainEvent;
