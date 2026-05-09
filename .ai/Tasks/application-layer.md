# Task: Implement Application Layer (VSA Slices) And Tests

**Branch:** `feature/application-layer`
**Ticket:** #4 - Implement Application layer with VSA slices and handler tests
**Pull Request:** #15 - feat(application): implement VSA Application layer with handler tests
**Base branch:** `main`

## Scope

Implement the full Application layer using Vertical Slice Architecture (VSA) and MediatR.
Includes all command/query slices for Products and Categories, common infrastructure
(behaviours, interfaces, exceptions, DTOs), domain event notification handlers, and
the MediatR bridge record. All handlers covered by Application.Tests (TDD — tests first).

Also adds missing domain pieces: Category aggregate, ICategoryRepository,
CategoryCreated event, and ProductDeleted event (needed by DeleteProduct slice).

## Material Changes

- `backend/ProductCatalog.Domain/Categories/*` — Category aggregate + events + repo interface
- `backend/ProductCatalog.Domain/Products/Events/ProductDeleted.cs`
- `backend/ProductCatalog.Application/Common/*`
- `backend/ProductCatalog.Application/Features/Products/*`
- `backend/ProductCatalog.Application/Features/Categories/*`
- `backend/ProductCatalog.Application/Handlers/*`
- `tests/ProductCatalog.Application.Tests/*`

## Acceptance Criteria

- [ ] All product command handlers (Create, Update, Delete, AdjustStock, Discontinue) tested
- [ ] All product query handlers (GetProductById, GetProducts, SearchProducts) tested
- [ ] All category handlers (CreateCategory, GetCategories, GetCategoryTree) tested
- [ ] ValidationBehaviour tested
- [ ] SearchCacheInvalidator tested
- [ ] `dotnet test` passes with all application handler tests
- [ ] PR checks pass
- [ ] Merged to `main`

## Progress

- [x] Feature branch created
- [x] Task file created
- [x] Missing domain pieces added (Category, ProductDeleted)
- [x] Failing tests written
- [x] Application Common implemented
- [x] Application Features implemented
- [x] Application Handlers implemented
- [x] Local tests pass (56 tests: 22 domain + 34 application)
- [x] Pull request opened (#15)
- [ ] PR checks pass
- [ ] Merged to `main`

## Risks / Blocks

- Category aggregate was out of scope for product-domain ticket — must be added here.
- ProductDeleted domain event missing — must be added here before DeleteProduct slice.
- Domain project must remain package-free throughout.

## Next Steps

After Application layer: Implement Infrastructure (EF InMemory, repos, dispatcher, seeder).
