.PHONY: install dev dev-backend dev-bff dev-ui test test-watch clean

# ── First-time setup ──────────────────────────────────────────────────
install:
	@echo "→ Restoring .NET packages..."
	dotnet restore ProductCatalog.sln
	@echo "→ Installing Angular dependencies..."
	cd frontend/product-catalog-ui && npm ci
	@echo "✓ Install complete. Run 'make dev' to start."

# ── Run all three services (requires parallel shell support) ─────────
dev:
	@echo "→ Starting all services. Logs will appear below."
	@echo "   API   → http://localhost:5000"
	@echo "   BFF   → http://localhost:5100"
	@echo "   UI    → http://localhost:4200"
	@$(MAKE) -j3 dev-backend dev-bff dev-ui

dev-backend:
	dotnet run --project backend/ProductCatalog.Api/ProductCatalog.Api.csproj \
		--launch-profile Development

dev-bff:
	dotnet run --project frontend/ProductCatalog.Bff/ProductCatalog.Bff.csproj \
		--launch-profile Development

dev-ui:
	cd frontend/product-catalog-ui && ng serve --proxy-config proxy.conf.json

# ── Tests ─────────────────────────────────────────────────────────────
test:
	dotnet test ProductCatalog.sln --no-restore --verbosity normal

test-watch:
	dotnet watch test --project tests/ProductCatalog.Domain.Tests

test-e2e:
	cd frontend/product-catalog-ui && npm run e2e

# ── Clean ─────────────────────────────────────────────────────────────
clean:
	dotnet clean ProductCatalog.sln
	find . -name "bin" -o -name "obj" | xargs rm -rf
	rm -rf frontend/product-catalog-ui/node_modules
	rm -rf frontend/product-catalog-ui/dist
