# Architecture

## Layer Diagram
```text
backend/  (project references)
  Domain <- Application <- Infrastructure <- Api

frontend/  (HTTP boundary - no project references to backend)
  Angular -> BFF -> [YARP/HTTP] -> Api
```

## Key Decisions
See decisions/ folder for individual ADRs.

## Domain Layer
Pure C#. Zero NuGet packages. Absolute zero - no MediatR, no nothing.
Aggregates: Product, Category.
Value Objects: ProductName, Sku, Money, StockQuantity.
Domain Events: ProductCreated, ProductUpdated, StockLevelChanged,
               ProductDiscontinued, ProductReactivated, CategoryCreated.

## Application Layer
VSA (Vertical Slice Architecture). Each feature is a self-contained folder.
CQRS via MediatR. Commands return DTOs. Queries return DTOs.
Domain event handlers live in `Application/Handlers/` - NOT inside feature slices.
Events are cross-cutting; handlers are not owned by the slice that raised the event.
ProductSearchEngine: BCL only, lives in Features/Products/SearchProducts/ - move to Common if a second slice needs it.

## Infrastructure Layer
EF Core 9 InMemory. RepositoryBase<T,TId> generic base.
SearchCacheRepository: pure Dictionary<string, CacheEntry> - no IMemoryCache.
DomainEventDispatcher: dispatches after SaveChangesAsync via MediatR.Publish.
EnsureCreated() on startup. SeedData config-toggled. No migrations.

## API Layer
Thin controllers. Custom RequestTimingMiddleware (IMiddleware, from scratch).
Custom SkuJsonConverter (JsonConverter<Sku>). Manual model binding on GET /products.
MediatR pipeline: LoggingBehaviour -> ValidationBehaviour -> Handler.

## BFF Layer
YARP proxies /api/* to Api. Custom thin BFF - no third-party BFF framework.
Single activation point for auth (Program.cs comments mark all changes needed).
Serves Angular static files in production.

## Angular SPA
Standalone components only. No NgModules. Reactive forms. RxJS pipes.
Tailwind CSS only - no component library. Search: debounce 300ms, distinctUntilChanged.
All HTTP via product.service.ts / category.service.ts -> BFF /api/*.
