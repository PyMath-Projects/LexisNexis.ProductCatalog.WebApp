#!/usr/bin/env bash
set -e

echo "Starting ProductCatalog dev environment..."
echo "API  → http://localhost:5000"
echo "BFF  → http://localhost:5100"
echo "UI   → http://localhost:4200"
echo ""
echo "Press Ctrl+C to stop all services."
echo ""

cleanup() {
  echo "Stopping services..."
  kill $(jobs -p) 2>/dev/null
  exit 0
}
trap cleanup SIGINT SIGTERM

# 1. Start API first
dotnet run --project backend/ProductCatalog.Api/ProductCatalog.Api.csproj &

# 2. Wait for API health before starting BFF and UI
echo "Waiting for API to be ready..."
until curl -sf http://localhost:5000/health >/dev/null 2>&1; do sleep 1; done
echo "✓ API ready."

# 3. Start BFF then UI
dotnet run --project frontend/ProductCatalog.Bff/ProductCatalog.Bff.csproj &
sleep 2
cd frontend/product-catalog-ui && ng serve --proxy-config proxy.conf.json &

wait
