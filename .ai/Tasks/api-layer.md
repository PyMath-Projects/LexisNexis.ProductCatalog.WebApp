# Task: Implement API Layer

**Branch:** `feature/api-layer`
**Ticket:** #7 - Implement API controllers, middleware, model binding, and serialization
**Pull Request:** (pending)
**Base branch:** `main`

## Scope

- `RequestTimingMiddleware` — IMiddleware logging request elapsed ms
- `SkuJsonConverter` — custom JsonConverter<Sku> for JSON serialization
- `ProductFilterModelBinder` — IModelBinder reading query params manually
- `ProductsController` — thin dispatch-only controller (7 endpoints)
- `CategoriesController` — thin dispatch-only controller (3 endpoints)
- `Program.cs` — MediatR + Infrastructure DI, pipeline behaviours, exception handler
- `appsettings.json` / `appsettings.Development.json` — Database:SeedData config
- `Properties/launchSettings.json` — port 5000

## Acceptance Criteria

- [ ] `dotnet build ProductCatalog.sln --configuration Release` passes
- [ ] `dotnet test ProductCatalog.sln --configuration Release` passes (56 tests)
- [ ] PR checks pass
- [ ] Merged to `main`

## Progress

- [x] Feature branch created
- [x] Task file created
- [ ] Api layer implemented
- [ ] Build verified
- [ ] Tests verified
- [ ] Pull request opened
- [ ] PR checks pass
- [ ] Merged to `main`

## Next Steps

After Api: Implement BFF (Program.cs, appsettings, YARP proxy config).
