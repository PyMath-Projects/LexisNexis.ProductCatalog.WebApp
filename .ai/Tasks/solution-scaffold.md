# Task: Scaffold Solution Structure And Project References

**Branch:** `feature/solution-scaffold`
**Ticket:** #1 - Scaffold solution structure and project references
**Base branch:** `main`

## Scope

Create the .NET solution, backend projects, BFF project, test projects, project references,
and package references required by the project brief.

## Material Changes

- `ProductCatalog.sln`
- `backend/ProductCatalog.Domain/ProductCatalog.Domain.csproj`
- `backend/ProductCatalog.Application/ProductCatalog.Application.csproj`
- `backend/ProductCatalog.Infrastructure/ProductCatalog.Infrastructure.csproj`
- `backend/ProductCatalog.Api/ProductCatalog.Api.csproj`
- `frontend/ProductCatalog.Bff/ProductCatalog.Bff.csproj`
- `tests/ProductCatalog.Domain.Tests/ProductCatalog.Domain.Tests.csproj`
- `tests/ProductCatalog.Application.Tests/ProductCatalog.Application.Tests.csproj`

## Acceptance Criteria

- [ ] `dotnet build ProductCatalog.sln` succeeds with zero warnings
- [ ] Project references match the dependency flow in the brief
- [ ] Nullable enable + TreatWarningsAsErrors in all backend `.csproj` files
- [ ] Domain project has zero package references
- [ ] Changes are committed incrementally

## Progress

- [x] Feature branch created
- [x] Task file created
- [ ] Solution created
- [ ] Projects created
- [ ] Project references added
- [ ] Package references added
- [ ] Build verified
- [ ] Branch pushed

## Risks / Blocks

- Package restore requires network access.
- Angular project scaffolding is intentionally out of scope for this ticket.

## Next Steps

- Scaffold the .NET solution and projects.
- Add references and exact package families required by the brief.
- Run restore/build and fix warnings before committing.
