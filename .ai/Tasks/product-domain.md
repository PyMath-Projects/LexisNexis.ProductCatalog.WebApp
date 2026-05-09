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
- [ ] Failing tests written
- [ ] Product domain model implemented
- [ ] Local tests pass
- [ ] Pull request opened
- [ ] PR checks pass
- [ ] Merged to `main`

## Verification

- Pending

## Risks / Blocks

- Domain project must remain package-free.
- Category aggregate is out of scope for this ticket except for repository interface dependency boundaries.

## Next Steps

- Write Product, Sku, Money, and StockQuantity tests first.
- Run tests to confirm the expected compile/test failure.
- Implement the domain model.
