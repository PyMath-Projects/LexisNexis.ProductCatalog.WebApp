# Task: Add CI Workflow And Branching Rules

**Branch:** `feature/solution-scaffold`
**Ticket:** #11 - Add GitHub Actions CI and branch workflow guidance
**Base branch:** `main`

## Scope

Add the repository automation and working convention that keeps `main` as the integration
branch while normal implementation happens on focused `feature/*` and `fix/*` branches.
On successful pushes to `main`, create a versioned GitHub Release.

## Material Changes

- `.github/workflows/ci.yml` - restore, build, test, tag, and release checks
- `AGENTS.md` - branch and CI conventions
- `.ai/Decisions.md` - coordination decision for CI-gated integration
- GitHub repository settings - branch protection for `main` after CI exists

## Initial CI Shape

The first workflow should run on pull requests and pushes to `main`:

- Checkout repository
- Install .NET 9 SDK
- `dotnet restore ProductCatalog.sln`
- `dotnet build ProductCatalog.sln --configuration Release --no-restore`
- `dotnet test ProductCatalog.sln --configuration Release --no-build --verbosity normal`
- On push to `main`, create tag/release `v0.1.<github-run-number>`

## Later CI Extension

After Angular is scaffolded:

- Install Node 18+
- Run `npm ci` in `frontend/product-catalog-ui`
- Run `npm run build`

## Progress

- [x] Branching convention recorded in `.ai/Decisions.md`
- [x] Branching convention recorded in AGENTS.md
- [x] GitHub issue created
- [x] `.github/workflows/ci.yml` created
- [x] Main-branch versioned release step added
- [x] Initial workflow commands pass locally
- [ ] Initial workflow passes on GitHub
- [ ] First main-branch release is created after merge
- [ ] Branch protection configured for `main`

## Verification

- `dotnet build ProductCatalog.sln --configuration Release --no-restore` - passed with 0 warnings and 0 errors
- `dotnet test ProductCatalog.sln --configuration Release --no-build --verbosity normal` - passed
- Release workflow syntax reviewed locally; release step is gated to push events on `refs/heads/main`

## Risks / Blocks

- CI could not pass until the solution was scaffolded; that scaffold now exists on this branch.
- Angular build checks must wait until the SPA exists.
- Release version currently uses `v0.1.<github-run-number>` until the project needs manual semantic version promotion.

## Next Steps

- Push this branch and verify the GitHub Actions run.
- After merge to `main`, verify a tag and GitHub Release are created.
- Configure branch protection for `main` after the workflow is visible in GitHub.
