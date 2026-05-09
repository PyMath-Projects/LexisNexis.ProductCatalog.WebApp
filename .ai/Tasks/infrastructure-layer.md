# Task: Implement Infrastructure Layer

**Branch:** `feature/infrastructure-layer`
**Ticket:** #5 - Implement Infrastructure (EF Core InMemory, repos, dispatcher, seeder)
**Pull Request:** (pending)
**Base branch:** `main`

## Scope

Implement the full Infrastructure layer:
- `ProductCatalogDbContext` with value-object EF mappings
- `RepositoryBase<T, TId>` generic base
- `ProductRepository` and `CategoryRepository`
- `DomainEventDispatcher` (post-save dispatch via MediatR reflection)
- `SearchCacheRepository` (pure Dictionary<string, object>)
- `DatabaseSeeder` (idempotent seed on startup)
- `DependencyInjection.cs` (AddInfrastructure + InitialiseDatabaseAsync)

## Material Changes

- `backend/ProductCatalog.Infrastructure/Persistence/*`
- `backend/ProductCatalog.Infrastructure/DependencyInjection.cs`

## Acceptance Criteria

- [ ] `dotnet build ProductCatalog.sln --configuration Release` passes with 0 warnings
- [ ] `dotnet test ProductCatalog.sln --configuration Release` passes (56 tests)
- [ ] Domain and Application NuGet constraints remain unchanged
- [ ] PR checks pass
- [ ] Merged to `main`

## Progress

- [x] Feature branch created
- [x] Task file created
- [ ] Infrastructure implemented
- [ ] Build verified
- [ ] Tests verified
- [ ] Pull request opened
- [ ] PR checks pass
- [ ] Merged to `main`

## Next Steps

After Infrastructure: Implement Api (controllers, middleware, Program.cs).
