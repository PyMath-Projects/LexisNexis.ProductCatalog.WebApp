# ADR-001: EF Core InMemory Provider (Spec Compliant)

## Status: Accepted

## Context
The spec states: "Use Entity Framework Core with in-memory database for most data access,
except where specified." We need to choose our persistence provider.

## Decision
Use EF Core's InMemory provider (`UseInMemoryDatabase`).

## Rationale
- Spec explicitly requires it
- No migrations needed - `EnsureCreated()` is sufficient
- Data resets on restart - acceptable and useful for a demo (inspectors get clean state)
- Simpler DI setup, simpler startup, simpler README
- The separate "pure in-memory collection" requirement (Dictionary-based SearchCache)
  is satisfied independently of the ORM provider

## Trade-offs Accepted
- No referential integrity enforcement (EF InMemory doesn't enforce FK constraints)
- LINQ translation behaviour differs slightly from production SQL providers
- Data does not persist across restarts

## Future Path
If persistence is needed: swap `UseInMemoryDatabase("ProductCatalog")` for
`UseSqlite("Data Source=catalog.db")` in `DependencyInjection.cs` and add migrations.
No other code changes required.
