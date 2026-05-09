# Architecture - Product Catalog

## Planned Repository Shape
```text
ProductCatalog/
  backend/
    ProductCatalog.Domain/
    ProductCatalog.Application/
    ProductCatalog.Infrastructure/
    ProductCatalog.Api/
  frontend/
    ProductCatalog.Bff/
    product-catalog-ui/
  tests/
    ProductCatalog.Domain.Tests/
    ProductCatalog.Application.Tests/
  .ai/
    Project.md
    Architecture.md
    Decisions.md
    Tasks/
  docs/
    ProjectBrief.md
```

## Dependency Direction
The dependency graph is intentional and must not be violated:
- `Domain -> none`
- `Application -> Domain`
- `Infrastructure -> Application, Domain`
- `Api -> Application, Infrastructure`
- `BFF -> none of backend projects`
- `Angular -> BFF over HTTP only`

```text
backend/  (project references)
  Domain <- Application <- Infrastructure <- Api

frontend/  (HTTP boundary - no project references to backend)
  Angular -> BFF -> [YARP/HTTP] -> Api
```

## Layer Responsibilities

### Domain
- Pure C# with zero NuGet packages
- Aggregates: Product, Category
- Value Objects: ProductName, Sku, Money, StockQuantity
- Domain Events: ProductCreated, ProductUpdated, StockLevelChanged,
  ProductDiscontinued, ProductReactivated, CategoryCreated
- Business invariants live in aggregate methods and value object factories

### Application
- Vertical Slice Architecture under `Features/`
- CQRS through MediatR
- Commands return DTOs and orchestrate writes
- Queries use direct projections and never load aggregates just to read data
- Domain event handlers live in `Application/Handlers/`
- Pipeline behaviours handle logging and pattern-matching validation

### Infrastructure
- EF Core 9 InMemory provider
- `RepositoryBase<T,TId>` generic repository base
- Dictionary-backed `SearchCacheRepository` with TTL and prefix invalidation
- `DomainEventDispatcher` publishes domain events after persistence
- Startup uses `EnsureCreatedAsync`; no migrations
- Seed data is config-controlled and idempotent

### API
- Thin ASP.NET Core controllers that dispatch only through MediatR
- Custom `RequestTimingMiddleware`
- Custom `SkuJsonConverter`
- Manual model binding for product filters on `GET /api/products`
- Database initialization runs during API startup

### BFF
- Custom thin ASP.NET Core BFF
- YARP proxies `/api/*` to ProductCatalog.Api
- Antiforgery endpoint at `/bff/antiforgery`
- Development CORS for Angular
- Production static file serving for Angular build output
- Auth extension points remain commented until needed

### Angular SPA
- Angular standalone components only
- Reactive Forms
- RxJS search with `debounceTime(300ms)` and `distinctUntilChanged`
- Tailwind CSS only; no component library
- All HTTP goes through BFF `/api/*`

## Request Handling Pattern
Write path:
1. HTTP POST/PUT/DELETE reaches API controller.
2. Controller sends command through MediatR.
3. Validation and logging behaviours run.
4. Handler loads aggregate through repository, calls one aggregate method, saves.
5. Domain events are dispatched after save.
6. Event handlers react, such as invalidating search cache.

Read path:
1. HTTP GET reaches API controller.
2. Controller sends query through MediatR.
3. Query handler uses direct EF projection.
4. DTO is returned without loading aggregate objects.

## AI Context
- `.ai/Project.md` describes project purpose and map.
- `.ai/Architecture.md` describes current/planned architecture.
- `.ai/Decisions.md` is the single living decisions file.
- `.ai/Tasks/` contains active task handoff files and must be updated while work is in progress.
