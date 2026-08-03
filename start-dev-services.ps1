# Start all required services for local development
Write-Host "Starting BusinessAsUsual local development services..." -ForegroundColor Cyan

# Kill any existing processes on these ports
$ports = @(5100, 5142, 5143, 5004, 5041, 5006, 5300)
foreach ($port in $ports) {
	$process = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue | Select-Object -ExpandProperty OwningProcess -Unique
	if ($process) {
		Write-Host "Killing process on port $port..." -ForegroundColor Yellow
		Stop-Process -Id $process -Force -ErrorAction SilentlyContinue
	}
}

Start-Sleep -Seconds 2

# Start Module Registry (required for all modules)
Write-Host "`nStarting ModuleRegistry.API on port 5100..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'services/ModuleRegistry/ModuleRegistry.API'; dotnet run"

Start-Sleep -Seconds 3

# Start Inventory API
Write-Host "Starting Inventory.API on port 5142..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'services/Inventory/Inventory.API'; dotnet run"

# Start Sales API
Write-Host "Starting Sales.API on port 5143..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'services/Sales/Sales.API'; dotnet run"

# Start CRM API (if needed)
Write-Host "Starting CRM.API on port 5004..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'services/CRM/CRM.API'; dotnet run"

# Start HR API (if needed)
Write-Host "Starting HR.API on port 5041..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'services/HR/HR.API'; dotnet run"

# Start Finance API (if needed)
Write-Host "Starting Finance.API on port 5006..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'services/Finance/Finance.API'; dotnet run"

# Start AI API (if needed)
Write-Host "Starting AI.Api on port 5300..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'services/AI/AI.Api'; dotnet run"

Write-Host "`n✓ All services starting..." -ForegroundColor Cyan
Write-Host "Services will open in separate PowerShell windows." -ForegroundColor Cyan
Write-Host "Close those windows or press Ctrl+C in them to stop services." -ForegroundColor Yellow
Write-Host "`nYou can now run BusinessAsUsual.Web from Visual Studio." -ForegroundColor Green
