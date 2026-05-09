Write-Host "Starting ProductCatalog dev environment..." -ForegroundColor Cyan
Write-Host "API  -> http://localhost:5000"
Write-Host "BFF  -> http://localhost:5100"
Write-Host "UI   -> http://localhost:4200"
Write-Host ""
Write-Host "Close terminal windows or press Ctrl+C to stop."

Start-Process powershell -ArgumentList `
  "-NoExit", "-Command", `
  "dotnet run --project backend/ProductCatalog.Api/ProductCatalog.Api.csproj"

Start-Sleep 3
Start-Process powershell -ArgumentList `
  "-NoExit", "-Command", `
  "dotnet run --project frontend/ProductCatalog.Bff/ProductCatalog.Bff.csproj"

Start-Sleep 2
Start-Process powershell -ArgumentList `
  "-NoExit", "-Command", `
  "cd frontend/product-catalog-ui; ng serve --proxy-config proxy.conf.json"

Write-Host "All services starting. Opening browser in 10 seconds..."
Start-Sleep 10
Start-Process "http://localhost:4200"
