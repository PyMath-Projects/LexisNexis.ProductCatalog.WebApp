# Claude-Specific Instructions

## Working Pattern
- Read the brief. Then re-read Section 4 (domain) before writing domain code.
- When implementing a handler: load -> call one domain method -> save -> dispatch.
  Never more than that.
- When in doubt about a design: check the PlantUML diagrams in the brief.

## Code Generation Rules
- Always include XML doc comments on public domain methods explaining the invariant.
- Always include the CancellationToken parameter on async methods.
- Always use `private set` - never auto-properties with public setters in aggregates.
- When generating a test: use FluentAssertions `.Should()` syntax.
- When generating a ValueObject: implement `GetEqualityComponents()` and
  both `==` and `!=` operators (inherited from ValueObject base).

## What to Avoid
- Do not suggest IMemoryCache - the spec requires Dictionary and we honour that.
- Do not suggest AutoMapper - mappings are explicit per-slice.
- Do not add any NuGet packages to Domain. Zero. IDomainEvent is a plain C# interface.
- Do not add `[ApiController]` model validation to the action that uses manual binding.
