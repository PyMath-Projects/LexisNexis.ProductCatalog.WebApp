# Product Catalog

A full-stack product catalog management system built as a senior C#/Angular developer assessment, demonstrating Domain-Driven Design, Vertical Slice Architecture, CQRS, Domain Events, and a thin BFF pattern.

## Stack

| Layer | Technology |
|---|---|
| Domain | C# / .NET 9 — pure, zero NuGet |
| Application | MediatR 12 · CQRS · VSA |
| Infrastructure | EF Core 9 InMemory · Dictionary search cache |
| API | ASP.NET Core 9 — thin controllers |
| BFF | ASP.NET Core 9 · YARP |
| Frontend | Angular 17 · TypeScript strict · Tailwind CSS · RxJS |

## Prerequisites

```bash
dotnet --version   # 9.0+
node --version     # 18+
npm --version      # 9+
ng version         # Angular CLI — install: npm i -g @angular/cli
```

## Quick Start

```bash
# Install all dependencies
make install

# Start all three services in parallel
make dev
```

Then open **http://localhost:4200**.

| Service | URL |
|---|---|
| Angular SPA | http://localhost:4200 |
| BFF | http://localhost:5100 |
| API | http://localhost:5000 |
| API health | http://localhost:5000/health |
| BFF health | http://localhost:5100/health |

## Without Make (Windows)

```powershell
scripts\start-dev.ps1
```

```bash
# Unix/Mac
./scripts/start-dev.sh
```

## Running Tests

```bash
# Backend unit tests (56 tests — Domain + Application)
make test

# Watch mode
make test-watch

# E2E (requires all three services running)
make test-e2e
```

## Project Structure

```
ProductCatalog/
├── backend/
│   ├── ProductCatalog.Domain/          # Pure domain — zero NuGet
│   ├── ProductCatalog.Application/     # MediatR VSA slices
│   ├── ProductCatalog.Infrastructure/  # EF InMemory, repos, dispatcher
│   └── ProductCatalog.Api/             # Thin ASP.NET Core API :5000
├── frontend/
│   ├── ProductCatalog.Bff/             # YARP thin BFF :5100
│   └── product-catalog-ui/             # Angular 17 SPA :4200
├── tests/
│   ├── ProductCatalog.Domain.Tests/
│   └── ProductCatalog.Application.Tests/
├── scripts/
│   ├── start-dev.sh
│   └── start-dev.ps1
├── docs/
│   └── ProjectBrief.md
├── Makefile
├── README.md
└── SOLUTION.md
```

## API Endpoints

All requests from the SPA go through the BFF at `:5100`, which proxies `/api/*` to the API at `:5000`.

| Method | Path | Description |
|---|---|---|
| GET | `/api/products` | List products (supports `?search=` and `?categoryId=`) |
| GET | `/api/products/{id}` | Get product by ID |
| POST | `/api/products` | Create product |
| PUT | `/api/products/{id}` | Update product |
| DELETE | `/api/products/{id}` | Delete product |
| PATCH | `/api/products/{id}/stock` | Adjust stock (`{ delta: int }`) |
| POST | `/api/products/{id}/discontinue` | Discontinue product |
| GET | `/api/categories` | List categories |
| GET | `/api/categories/tree` | Get category tree |
| POST | `/api/categories` | Create category |

## Seed Data

The database is in-memory and seeds on every API startup (config-controlled via `Database:SeedData`). Inspectors always get a clean, predictable state.
