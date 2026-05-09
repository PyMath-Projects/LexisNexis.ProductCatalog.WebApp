# Agent Instructions - ProductCatalog

## Execution Order
1. Read `docs/ProjectBrief.md` fully before starting any task.
2. Create the `.ai/` directory with all files defined in Section 10 of the brief.
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

## Test-First Rule
For every class in Domain and every Handler in Application:
write the failing test before writing the implementation.

## File Naming
- One class/record per file
- File name matches class name exactly
- Events in Events/ subfolder of their aggregate
- Slice folders use PascalCase matching the command/query name
- Domain event handlers go in Application/Handlers/ - never inside a feature slice

## Git Commit Convention
feat(domain): add Product aggregate
feat(application): add CreateProduct slice
feat(infra): add ProductRepository
feat(api): add ProductsController
feat(bff): wire YARP proxy
feat(ui): add product-list component
test(domain): Product.Create raises ProductCreated
fix: correct stock quantity adjustment invariant
