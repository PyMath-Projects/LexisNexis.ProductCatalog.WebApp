# ADR-004: In-Process Domain Events

## Status: Accepted

## Context
Aggregates need to signal important business facts such as product creation, stock changes,
and discontinuation. The demo scope does not require integration with external systems or
a message broker.

## Decision
Raise plain domain events inside aggregates and dispatch them in-process after persistence
using MediatR. The Domain project remains unaware of MediatR; the Application layer provides
`DomainEventNotification<T>` as the bridge from domain events to MediatR notifications.

## Rationale
- Keeps business events close to the aggregate state changes that cause them
- Preserves a pure Domain project with zero NuGet dependencies
- Decouples command handlers from reactions such as cache invalidation and stock alerts
- Keeps implementation simple and testable for the assessment scope
- Leaves a clear upgrade path to an outbox if external integration is needed later

## Trade-offs Accepted
- Event reactions run in-process and are not durable across process failure
- Event dispatch failures happen after the data commit and need explicit handling if they become critical
- This is not a cross-service integration mechanism

## Future Path
If another bounded context must consume events, domain event handlers can write integration
events to an outbox table in the same transaction as the domain change. A background outbox
processor can later publish those messages to a broker.
