# Task: Scaffold Solution Structure And Project References

**Branch:** `feature/solution-scaffold`
**Ticket:** #1 - Scaffold solution structure and project references
**Pull Request:** #12 - Scaffold solution structure and CI
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

- [x] `dotnet build ProductCatalog.sln` succeeds with zero warnings
- [x] Project references match the dependency flow in the brief
- [x] Nullable enable + TreatWarningsAsErrors in all backend `.csproj` files
- [x] Domain project has zero package references
- [x] Changes are committed incrementally

## Progress

- [x] Feature branch created
- [x] Task file created
- [x] Solution created
- [x] Projects created
- [x] Project references added
- [x] Package references added
- [x] Build verified
- [x] Tests verified
- [x] CI workflow added
- [x] Branch pushed
- [x] Pull request opened
- [x] PR checks passed
- [x] Merged to `main`

## Verification

- `dotnet restore ProductCatalog.sln` - passed
- `dotnet build ProductCatalog.sln --no-restore` - passed with 0 warnings and 0 errors
- `dotnet test ProductCatalog.sln --no-build --verbosity normal` - passed

## Risks / Blocks

- Package restore requires network access.
- Angular project scaffolding is intentionally out of scope for this ticket.

## Next Steps

- Continue with domain tests and implementation on the next feature branch.
