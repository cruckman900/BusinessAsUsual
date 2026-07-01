# Multi-Service Startup Script with Port Checking
# Starts all required services for theme sync testing

Write-Host "Starting BusinessAsUsual Multi-Service Environment" -ForegroundColor Cyan
Write-Host ""

# Define service configuration
$services = @(
	@{ Name = "ModuleRegistry"; Path = "services\ModuleRegistry\ModuleRegistry.API"; Port = 5100 }
	@{ Name = "HR.API"; Path = "services\HR\HR.API"; Port = 5001 }
	@{ Name = "HR.Web"; Path = "services\HR\HR.Web"; Port = 5002 }
	@{ Name = "Web Shell"; Path = "frontend\BusinessAsUsual.Web"; Port = 5269 }
)

# Function to check if port is in use
function Test-PortInUse {
	param([int]$Port)

	$connection = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue
	return $null -ne $connection
}

# Check for port conflicts
Write-Host "Checking for port conflicts..." -ForegroundColor Yellow
$hasConflicts = $false

foreach ($service in $services) {
	if (Test-PortInUse -Port $service.Port) {
		Write-Host "  ERROR: Port $($service.Port) ($($service.Name)) is already in use!" -ForegroundColor Red
		$hasConflicts = $true
	} else {
		Write-Host "  OK: Port $($service.Port) ($($service.Name)) is available" -ForegroundColor Green
	}
}

if ($hasConflicts) {
	Write-Host ""
	Write-Host "Port conflicts detected! Please stop the conflicting services:" -ForegroundColor Red
	Write-Host "  1. Close Visual Studio debugger if running" -ForegroundColor Yellow
	Write-Host "  2. Stop any dotnet processes: Get-Process dotnet | Stop-Process -Force" -ForegroundColor Yellow
	Write-Host "  3. Or use different ports in the script" -ForegroundColor Yellow
	Write-Host ""
	Write-Host "To find what's using a port:" -ForegroundColor Cyan
	Write-Host "  Get-NetTCPConnection -LocalPort 5269 | Select OwningProcess" -ForegroundColor Gray
	Write-Host "  Get-Process -Id <ProcessId>" -ForegroundColor Gray
	exit 1
}

Write-Host ""
Write-Host "All ports available! Starting services..." -ForegroundColor Green
Write-Host ""

# Start services
$jobs = @()

for ($i = 0; $i -lt $services.Count; $i++) {
	$service = $services[$i]
	$num = $i + 1

	Write-Host "[$num/$($services.Count)] Starting $($service.Name) on port $($service.Port)..." -ForegroundColor Yellow

	$job = Start-Job -ScriptBlock {
		param($path, $port)
		Set-Location $path
		dotnet run --urls "http://localhost:$port"
	} -ArgumentList (Join-Path $PSScriptRoot $service.Path), $service.Port

	$jobs += @{ Job = $job; Name = $service.Name; Port = $service.Port }

	Start-Sleep -Seconds 3
}

Write-Host ""
Write-Host "Waiting for services to start..." -ForegroundColor Cyan
Start-Sleep -Seconds 10

Write-Host ""
Write-Host "Services Started!" -ForegroundColor Green
Write-Host ""
Write-Host "Service URLs:" -ForegroundColor White
Write-Host "   Module Registry: http://localhost:5100" -ForegroundColor Cyan
Write-Host "   HR API:          http://localhost:5001" -ForegroundColor Cyan
Write-Host "   HR Web:          http://localhost:5002" -ForegroundColor Cyan
Write-Host "   Parent Shell:    http://localhost:5269" -ForegroundColor Cyan
Write-Host ""
Write-Host "Testing Theme Sync:" -ForegroundColor White
Write-Host "   1. Open http://localhost:5269/dashboard" -ForegroundColor Gray
Write-Host "   2. Click 'HR' module card (loads iframe)" -ForegroundColor Gray
Write-Host "   3. Open browser DevTools console" -ForegroundColor Gray
Write-Host "   4. Look for '[ThemeSync]' and '[IframeThemeReceiver]' logs" -ForegroundColor Gray
Write-Host "   5. Change theme using theme switcher" -ForegroundColor Gray
Write-Host "   6. Verify HR module updates immediately" -ForegroundColor Gray
Write-Host ""
Write-Host "Press Ctrl+C to stop all services" -ForegroundColor Yellow
Write-Host ""

# Monitor job output in real-time
try {
	while ($true) {
		foreach ($jobInfo in $jobs) {
			Receive-Job -Job $jobInfo.Job -ErrorAction SilentlyContinue | ForEach-Object {
				$color = switch ($jobInfo.Name) {
					"ModuleRegistry" { "DarkMagenta" }
					"HR.API" { "DarkGreen" }
					"HR.Web" { "DarkYellow" }
					"Web Shell" { "DarkCyan" }
					default { "Gray" }
				}
				Write-Host "[$($jobInfo.Name)] $_" -ForegroundColor $color
			}
		}

		# Check if all jobs are still running
		$runningJobs = $jobs | Where-Object { $_.Job.State -eq 'Running' }

		if ($runningJobs.Count -lt $jobs.Count) {
			Write-Host "WARNING: One or more services stopped unexpectedly" -ForegroundColor Red
			Write-Host "Job States:" -ForegroundColor Yellow
			foreach ($jobInfo in $jobs) {
				Write-Host "  $($jobInfo.Name): $($jobInfo.Job.State)" -ForegroundColor Gray
			}
			break
		}

		Start-Sleep -Milliseconds 500
	}
}
finally {
	Write-Host ""
	Write-Host "Stopping services..." -ForegroundColor Red
	foreach ($jobInfo in $jobs) {
		Stop-Job -Job $jobInfo.Job -ErrorAction SilentlyContinue
		Remove-Job -Job $jobInfo.Job -Force -ErrorAction SilentlyContinue
	}
	Write-Host "All services stopped" -ForegroundColor Green
}
