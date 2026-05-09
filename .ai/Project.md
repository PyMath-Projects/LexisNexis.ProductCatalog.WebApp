# Product Catalog

## Purpose
A Product Catalog Management System built as a senior C#/Angular developer assessment.
The goal is to demonstrate production-grade architecture in a focused demo context.

## Business Domain
An e-commerce administrator manages a product catalog: creating products, organising them
into hierarchical categories, tracking stock levels, and searching the catalog.

## Key Constraints
- This is an assessment - scope is intentionally bounded
- Authentication is deliberately excluded; the BFF scaffold is ready for it later
- No external message broker - domain events are in-process
- EF Core InMemory - spec-compliant, no migrations required

## Non-Goals
- Payment processing
- Order management
- Authentication/authorisation beyond scaffolded extension points
- Multi-tenancy
- Real-time notifications through WebSocket or SignalR

## Stack
- Backend: .NET 9, ASP.NET Core, EF Core InMemory, MediatR
- Frontend: Angular 17, TypeScript, Tailwind CSS, RxJS
- BFF: .NET 9, YARP custom thin BFF
- Tests: xUnit, FluentAssertions, Moq

## Planned Solution Map
### Backend
- `backend/ProductCatalog.Domain/` - pure domain model, value objects, aggregates, domain events
- `backend/ProductCatalog.Application/` - MediatR, CQRS, VSA feature slices, pipeline behaviours
- `backend/ProductCatalog.Infrastructure/` - EF Core InMemory, repositories, search cache, domain event dispatcher
- `backend/ProductCatalog.Api/` - thin controllers, middleware, model binding, serialization

### Frontend
- `frontend/ProductCatalog.Bff/` - YARP proxy, antiforgery, health checks, production static file serving
- `frontend/product-catalog-ui/` - Angular standalone SPA

### Tests
- `tests/ProductCatalog.Domain.Tests/`
- `tests/ProductCatalog.Application.Tests/`

## Ports (Development)
- API: http://localhost:5000
- BFF: http://localhost:5100
- UI: http://localhost:4200

## Source Of Truth
- Product brief: `docs/ProjectBrief.md`
- Agent instructions: `AGENTS.md`
- Claude-specific guidance: `CLAUDE.md`
- Current active task context: `.ai/Tasks/`
- Repo issues: `PyMath-Projects/LexisNexis.ProductCatalog.WebApp`
- GitHub Project: `PyMath-Projects` project `12`
