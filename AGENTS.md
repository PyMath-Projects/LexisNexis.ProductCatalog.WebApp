# Agent Instructions - ProductCatalog

## Execution Order
1. Read `docs/ProjectBrief.md` fully before starting any task.
2. Maintain the `.ai/` directory (`Project.md`, `Architecture.md`, `Decisions.md`, `Tasks/`) as the active agent context.
3. Scaffold the solution: sln, projects, project references, NuGet packages.
4. Implement Domain + Domain.Tests (TDD - tests first).
5. Implement Application (VSA slices) + Application.Tests.
6. Implement Infrastructure (EF, repos, dispatcher, seeder).
7. Implement Api (controllers, middleware, program.cs).
8. Implement BFF (Program.cs, appsettings, proxy config).
9. Implement Angular SPA.
10. Create Makefile + startup scripts.
11. Write README.md and SOLUTION.md.

## Non-Negotiables
- Domain project: zero NuGet - including MediatR. Use the DomainEventNotification<T> bridge in Application
- ProductSearchEngine.cs: BCL only - no external packages
- No AutoMapper anywhere
- No public setters on aggregates
- No logic in controllers
- No NgModules in Angular
- No `any` type in TypeScript
- Nullable enable + TreatWarningsAsErrors in all backend .csproj files
- `main` is the integration branch; implementation work belongs on `feature/*` or `fix/*` branches
- GitHub Actions must pass before work is merged back to `main`

## Test-First Rule
For every class in Domain and every Handler in Application:
write the failing test before writing the implementation.

## File Naming
- One class/record per file
- File name matches class name exactly
- Events in Events/ subfolder of their aggregate
- Slice folders use PascalCase matching the command/query name
- Domain event handlers go in Application/Handlers/ - never inside a feature slice
- Decisions belong in the single `.ai/Decisions.md` file - do not create per-ADR files
- Active task handoff files belong in `.ai/Tasks/`

## Git Commit Convention
feat(domain): add Product aggregate
feat(application): add CreateProduct slice
feat(infra): add ProductRepository
feat(api): add ProductsController
feat(bff): wire YARP proxy
feat(ui): add product-list component
test(domain): Product.Create raises ProductCreated
fix: correct stock quantity adjustment invariant

## Branch And CI Convention
- Use `feature/<short-description>` for planned feature work.
- Use `fix/<short-description>` for corrections and regressions.
- Keep commits incremental and testable.
- Open PRs back to `main`; do not treat long-running local changes as complete until CI passes.
- Required GitHub Actions workflow: restore, build, and test .NET solution; add Angular install/build once the SPA is scaffolded.
- Successful pushes to `main` must create a versioned GitHub Release. Current version pattern: `v0.1.<github-run-number>`.
