# Decisions - Product Catalog

## Current Technical Decisions

- **Product catalog assessment scope**: this repository is a focused senior C#/Angular assessment. Keep scope bounded to catalog management, category hierarchy, stock handling, search, BFF, and demo-ready documentation.
- **Domain project has zero NuGet packages**: the Domain layer must not reference MediatR or any other package. Domain events are plain C# records implementing a plain `IDomainEvent`.
- **Rich domain model**: aggregates are not data bags. Business state changes go through named methods that enforce invariants and raise domain events. No public setters on aggregates.
- **Aggregate references by ID only**: Product references Category by `CategoryId`; aggregates must not hold object references to other aggregates.
- **Vertical Slice Architecture in Application**: application use cases live under `Features/{Context}/{UseCase}/`. Commands, queries, handlers, DTOs, and slice-local mappings stay with the slice.
- **CQRS read/write separation**: command handlers load aggregates through repositories and call domain methods. Query handlers use direct projections and do not load aggregates just to read data.
- **MediatR 12 for in-process dispatch**: Application uses `IRequest<T>` / `IRequestHandler<TRequest,TResponse>` for commands and queries, plus `INotificationHandler<T>` for domain event reactions.
- **Domain event bridge lives in Application**: `DomainEventNotification<T>` wraps plain domain events so Infrastructure can publish them through MediatR without Domain depending on MediatR.
- **Domain events dispatch after persistence**: data is committed first, then `DomainEventDispatcher` publishes events and clears them. If external integrations are added later, use an outbox.
- **Event handlers live outside feature slices**: domain event handlers are placed in `Application/Handlers/` and named `On{EventName}.cs` because events are broadcast facts, not owned by the slice that raised them.
- **EF Core InMemory provider**: Infrastructure uses `UseInMemoryDatabase("ProductCatalog")` per the brief. No migrations are used for this demo.
- **Database startup uses EnsureCreated**: API startup calls infrastructure initialization, which runs `EnsureCreatedAsync` and optionally seeds data based on configuration.
- **Search cache is a Dictionary**: cache implementation must be a pure Dictionary-backed repository with TTL and prefix invalidation. Do not use `IMemoryCache`.
- **ProductSearchEngine is BCL-only**: fuzzy product search uses only `System.*` namespaces. Do not add external search packages.
- **No AutoMapper**: mappings are explicit and slice-owned, usually as local extension methods.
- **Controllers stay thin**: API controllers dispatch through MediatR and do not contain business logic.
- **Manual model binding for product filters**: `GET /api/products` uses a custom model binder rather than default `[FromQuery]` validation.
- **Custom thin BFF**: the BFF is an ASP.NET Core app with YARP as its only external package. It owns proxying, antiforgery, health checks, CORS, auth extension points, and production static file serving.
- **Frontend/backend boundary is HTTP**: Angular talks only to BFF `/api/*`; BFF talks to API over HTTP. Frontend projects must not reference backend C# projects.
- **Angular standalone components only**: no NgModules. Use Reactive Forms, RxJS, Tailwind CSS, and strict TypeScript without `any`.
- **Incremental commits are required**: each stage should be committed after verification so the app remains testable and reviewable.
- **Main is the integration branch**: feature and fix work should happen on focused branches and merge back to `main` only after review/verification.
- **GitHub Actions gates integration**: CI should restore, build, and test the .NET solution; once Angular exists, CI should also install and build the SPA.

## Accepted Architecture Decisions

### ADR-001: EF Core InMemory Provider

Use EF Core's InMemory provider (`UseInMemoryDatabase`) because the brief explicitly requires it.

Rationale:
- No migrations needed; `EnsureCreated()` is sufficient.
- Data resets on restart, which is acceptable and useful for a demo.
- Setup remains simple for inspectors.
- The separate pure in-memory collection requirement is satisfied independently by the Dictionary search cache.

Trade-offs accepted:
- No referential integrity enforcement.
- LINQ translation behaviour can differ from production SQL providers.
- Data does not persist across restarts.

Future path:
If persistence is needed, swap to SQLite or another provider in Infrastructure and add migrations. The rest of the architecture should not need to change.

### ADR-002: Vertical Slice Architecture In Application

Organize Application by feature slice rather than by technical type.

Rationale:
- A developer working on one use case can stay in one folder.
- Each slice owns its request, handler, response DTO, and mapping.
- Shared concerns remain in `Common/`.
- Domain event handlers remain outside slices because event reactions are cross-cutting.

Consequences:
- Some DTOs may look similar across slices. That is intentional because slices own their response contracts.
- Shared code must not be hidden inside a feature folder unless only that slice uses it.

### ADR-003: Custom Thin BFF With YARP

Implement a custom ASP.NET Core BFF that uses YARP to proxy `/api/*` requests to the API.

Rationale:
- Keeps Angular isolated from API host and port details.
- Avoids a third-party BFF framework.
- Keeps future auth changes concentrated in the BFF.
- Maintains the required HTTP boundary between frontend and backend projects.

Trade-offs accepted:
- Local development needs three running processes: API, BFF, and Angular.
- The BFF initially contains little business-specific code.

Future path:
When authentication is required, add cookie/OIDC authentication, login/logout/user endpoints, authorization middleware, and authorization on the YARP route.

### ADR-004: In-Process Domain Events

Raise plain domain events inside aggregates and dispatch them in-process after persistence using MediatR.

Rationale:
- Keeps business events close to the aggregate state changes that cause them.
- Preserves a pure Domain project.
- Decouples command handlers from reactions such as cache invalidation and stock alerts.
- Keeps the demo implementation simple and testable.

Trade-offs accepted:
- Event reactions are not durable across process failure.
- Dispatch failures occur after data commit and need explicit handling if they become critical.
- This is not a cross-service integration mechanism.

Future path:
If another bounded context must consume events, domain event handlers can write integration events to an outbox table in the same transaction as the domain change. A background processor can publish those messages to a broker.

## Coordination Decisions
- Source of truth for what is implemented: the code in this repository.
- Source of truth for planned work: `docs/ProjectBrief.md`, GitHub issues, and GitHub Project tracking.
- Branch model: use `feature/*` branches for planned feature work and `fix/*` branches for corrections; `main` is the integration branch.
- GitHub Actions: add `.github/workflows/ci.yml` early in M0. Initial workflow should run on pushes and pull requests to `main`, install .NET 9, restore, build with warnings as errors, and run tests. Extend the workflow with Node/Angular install and build steps when `frontend/product-catalog-ui` is scaffolded.
- Task-file handoff: active files in `.ai/Tasks/` are live task status records and must be updated as scope, blockers, verification, commits, and next steps change.
- PR/readiness gate: before marking a task complete, all acceptance criteria and verification commands for that task should be recorded.
- Repo issues: `PyMath-Projects/LexisNexis.ProductCatalog.WebApp`.

## Documentation Rule For Agents
- If a concept exists in code, treat it as implemented.
- If a concept appears only in the brief or GitHub planning, treat it as planned until code exists.
- Do not rewrite architecture docs to describe future slices as already implemented.
- Keep `.ai/Decisions.md` as the single decisions file; do not create per-ADR markdown files.
