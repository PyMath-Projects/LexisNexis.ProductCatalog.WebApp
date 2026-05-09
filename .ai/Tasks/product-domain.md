# Task: Implement Product Aggregate And Value Objects

**Branch:** `feature/product-domain`
**Ticket:** #3 - Implement Product aggregate and value objects with tests
**Base branch:** `main`

## Scope

Implement the Product aggregate root, shared kernel, product value objects, product domain
events, and product repository interface using test-first development.

## Material Changes

- `backend/ProductCatalog.Domain/Shared/*`
- `backend/ProductCatalog.Domain/Products/*`
- `backend/ProductCatalog.Domain/Products/Events/*`
- `tests/ProductCatalog.Domain.Tests/Products/*`

## Acceptance Criteria

- [ ] Product.Create() raises exactly one ProductCreated domain event
- [ ] Stock adjustments enforce non-negative stock and status transitions
- [ ] Discontinue and Reactivate enforce lifecycle invariants
- [ ] Sku, Money, and StockQuantity value object tests pass
- [ ] Product.CompareTo() sorts by Price then Name
- [ ] `dotnet test` passes
- [ ] PR checks pass

## Progress

- [x] Feature branch created
- [x] Task file created
- [x] Failing tests written
- [x] Product domain model implemented
- [x] Local tests pass
- [ ] Pull request opened
- [ ] PR checks pass
- [ ] Merged to `main`

## Verification

- `dotnet test tests/ProductCatalog.Domain.Tests/ProductCatalog.Domain.Tests.csproj --no-restore` failed before implementation because Product domain classes did not exist.
- `dotnet test tests/ProductCatalog.Domain.Tests/ProductCatalog.Domain.Tests.csproj --no-restore` - passed after implementation, 22 domain tests.
- `dotnet build ProductCatalog.sln --no-restore` - passed with 0 warnings and 0 errors.
- `dotnet test ProductCatalog.sln --no-build --verbosity normal` - passed.
- `dotnet build ProductCatalog.sln --configuration Release --no-restore` - passed with 0 warnings and 0 errors.
- `dotnet test ProductCatalog.sln --configuration Release --no-build --verbosity normal` - passed.
- Confirmed `backend/ProductCatalog.Domain/ProductCatalog.Domain.csproj` has zero package references.

## Risks / Blocks

- Domain project must remain package-free.
- Category aggregate is out of scope for this ticket except for repository interface dependency boundaries.

## Next Steps

- Write Product, Sku, Money, and StockQuantity tests first.
- Run tests to confirm the expected compile/test failure.
- Implement the domain model.
