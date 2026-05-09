# Solution Notes

This document explains the key design decisions made during implementation.

## Architecture Summary

The system is split into two independent worlds joined only by HTTP:

```
Angular SPA :4200  →  BFF :5100  →  API :5000
                                         ↕
                                    Application (VSA)
                                         ↕
                                    Domain (pure C#)
                                         ↕
                                    Infrastructure (EF)
```

No project reference crosses the HTTP boundary. The BFF knows the API only as a URL in `appsettings.json`.

---

## Domain Layer

**No NuGet packages. Zero.** This is enforced by the `.csproj` file — zero `<PackageReference>` entries.

The domain uses a rich model: no public setters, no data bags. Every state change goes through a named business method that raises a domain event. For example, `Product.AdjustStock(delta)` validates the invariant (stock cannot go negative), updates the quantity, and raises `StockLevelChanged`. The handler never touches a field directly.

Value objects (`Sku`, `Money`, `ProductName`, `StockQuantity`) are immutable and self-validating via static factory methods. Invalid state cannot be constructed.

---

## Application Layer — Vertical Slice Architecture

Each feature is a self-contained folder:

```
Features/Products/CreateProduct/
  CreateProductCommand.cs
  CreateProductHandler.cs
  ProductDto.cs         ← this slice owns its response shape
```

Handlers follow a strict one-method pattern: **load → call one domain method → save → dispatch**. No business logic leaks into handlers.

Domain event handlers live in `Application/Handlers/` — not inside feature slices — because events are cross-cutting. `OnProductCreated` invalidates the search cache without the `CreateProduct` handler knowing or caring.

The MediatR pipeline runs: `LoggingBehaviour → ValidationBehaviour → Handler`.

---

## Infrastructure Layer

**EF Core InMemory** — spec-compliant, no migrations. `EnsureCreated()` on startup.

**SearchCacheRepository** uses a plain `Dictionary<string, CacheEntry>`, not `IMemoryCache`. This is a deliberate spec requirement demonstrating that caching can be implemented without framework abstractions.

**DomainEventDispatcher** publishes domain events *after* `SaveChangesAsync` succeeds — ensuring events only fire when the write is committed.

---

## API Layer

Controllers are thin dispatchers only. No logic lives in controllers.

Three custom components demonstrate manual framework extension:
- `RequestTimingMiddleware` — measures and logs request duration
- `SkuJsonConverter` — serialises/deserialises the `Sku` value object cleanly
- `ProductFilterModelBinder` — manually binds `GET /api/products` query parameters into a typed `ProductFilter` (no `[ApiController]` magic on that action)

---

## BFF Layer

The BFF is a **custom thin ASP.NET Core host** — not a BFF framework. Its only NuGet dependency is `Yarp.ReverseProxy`.

Responsibilities:
- Proxy `/api/*` to the API via YARP
- Issue the XSRF-TOKEN cookie at `/bff/antiforgery`
- Allow Angular dev server in CORS (`DevAngular` policy)
- Serve Angular static files in production

Auth extension points are commented in `Program.cs` — adding cookie auth, claims transformation, and logout requires changes in only one file and zero changes to the API or Angular.

---

## Angular SPA

**Standalone components only** — no NgModules anywhere. Angular 17's control flow syntax (`@if`, `@for`) is used throughout.

**Typed reactive forms** — `FormBuilder.nonNullable.group(...)` throughout the product form, so `getRawValue()` returns fully typed values with no `any`.

**Search** uses `debounceTime(300ms)` + `distinctUntilChanged()` via RxJS operators on a `FormControl`, wired through `combineLatest` with the category filter to produce a single stream that drives `switchMap` → HTTP call. Requests are automatically cancelled when a new search fires.

**Services** (`product.service.ts`, `category.service.ts`) are the only callers of `HttpClient`. Components inject services, not `HttpClient` directly.

---

## What Was Deliberately Left Out

| Item | Reason |
|---|---|
| Authentication | Assessment scope; BFF scaffold is ready for it |
| Pagination on the list | Not required by the brief |
| Real database | EF InMemory is spec-mandated |
| AutoMapper | Explicit per-slice mappings; brief prohibits it |
| Component library | Brief requires Tailwind only |
| NgModules | Brief requires standalone only |
| IMemoryCache | Brief requires Dictionary-backed cache |

---

## Running Everything

```bash
make install   # one-time setup
make dev       # starts API + BFF + UI in parallel
make test      # 56 backend unit tests
make test-e2e  # Playwright e2e against live stack
```
