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

dotnet run --project backend/ProductCatalog.Api/ProductCatalog.Api.csproj &
sleep 3
dotnet run --project frontend/ProductCatalog.Bff/ProductCatalog.Bff.csproj &
sleep 2
cd frontend/product-catalog-ui && ng serve --proxy-config proxy.conf.json &

wait
