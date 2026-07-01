# Service Management Script for BusinessAsUsual
# This script helps start, stop, and check the status of microservices

param(
	[Parameter(Mandatory=$false)]
	[ValidateSet('start', 'stop', 'status', 'cleanup')]
	[string]$Action = 'status'
)

$ErrorActionPreference = 'Continue'

# Service definitions
$Services = @{
	'ModuleRegistry' = @{
		Name = 'Module Registry Service'
		Path = 'services\ModuleRegistry\ModuleRegistry.API'
		Port = 5100
		Color = 'Cyan'
	}
	'HR' = @{
		Name = 'HR Service'
		Path = 'services\HR\HR.API'
		Port = 5041
		Color = 'Green'
	}
	'Web' = @{
		Name = 'Web Application'
		Path = 'frontend\BusinessAsUsual.Web'
		Port = 5269
		Color = 'Yellow'
	}
}

function Get-ServiceStatus {
	param([int]$Port)

	$connection = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue
	if ($connection) {
		$process = Get-Process -Id $connection.OwningProcess -ErrorAction SilentlyContinue
		return @{
			Running = $true
			ProcessId = $connection.OwningProcess
			ProcessName = $process.ProcessName
		}
	}
	return @{ Running = $false }
}

function Show-Status {
	Write-Host "`n=== BusinessAsUsual Services Status ===" -ForegroundColor White
	Write-Host ""

	foreach ($key in $Services.Keys | Sort-Object) {
		$service = $Services[$key]
		$status = Get-ServiceStatus -Port $service.Port

		Write-Host "$($service.Name):" -ForegroundColor $service.Color -NoNewline
		Write-Host " (Port $($service.Port))" -NoNewline

		if ($status.Running) {
			Write-Host " [RUNNING]" -ForegroundColor Green -NoNewline
			Write-Host " - PID: $($status.ProcessId) ($($status.ProcessName))"
		} else {
			Write-Host " [STOPPED]" -ForegroundColor Red
		}
	}

	Write-Host ""
}

function Stop-Services {
	Write-Host "`n=== Stopping All Services ===" -ForegroundColor Yellow
	Write-Host ""

	foreach ($key in $Services.Keys) {
		$service = $Services[$key]
		$status = Get-ServiceStatus -Port $service.Port

		if ($status.Running) {
			Write-Host "Stopping $($service.Name)..." -ForegroundColor $service.Color
			Stop-Process -Id $status.ProcessId -Force -ErrorAction SilentlyContinue
			Start-Sleep -Milliseconds 500
			Write-Host "  ✓ Stopped" -ForegroundColor Green
		} else {
			Write-Host "$($service.Name) is not running" -ForegroundColor Gray
		}
	}

	Write-Host ""
}

function Start-Services {
	Write-Host "`n=== Starting Services ===" -ForegroundColor Cyan
	Write-Host ""
	Write-Host "RECOMMENDED: Use Visual Studio GUI instead!" -ForegroundColor Yellow
	Write-Host "Right-click solution → Properties → Startup Project → Multiple startup projects" -ForegroundColor Yellow
	Write-Host ""
	Write-Host "This script starts services in separate windows." -ForegroundColor Gray
	Write-Host "Press Ctrl+C in each window to stop them." -ForegroundColor Gray
	Write-Host ""

	$workspaceRoot = "D:\DotNet Projects\BusinessAsUsual"

	foreach ($key in @('ModuleRegistry', 'HR', 'Web')) {
		$service = $Services[$key]
		$status = Get-ServiceStatus -Port $service.Port

		if ($status.Running) {
			Write-Host "$($service.Name) is already running on port $($service.Port)" -ForegroundColor Yellow
		} else {
			$fullPath = Join-Path $workspaceRoot $service.Path
			Write-Host "Starting $($service.Name)..." -ForegroundColor $service.Color

			Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$fullPath'; dotnet run" -WindowStyle Normal
			Start-Sleep -Seconds 2
		}
	}

	Write-Host ""
	Write-Host "Services are starting in separate windows..." -ForegroundColor Green
	Write-Host "Check each window for startup messages." -ForegroundColor Gray
	Write-Host ""
}

function Clean-Ports {
	Write-Host "`n=== Cleaning Up Ports ===" -ForegroundColor Yellow
	Write-Host ""

	$portsToCheck = @(5100, 5041, 5269)

	foreach ($port in $portsToCheck) {
		$connection = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue
		if ($connection) {
			$process = Get-Process -Id $connection.OwningProcess -ErrorAction SilentlyContinue
			Write-Host "Port $port is in use by $($process.ProcessName) (PID: $($process.Id))" -ForegroundColor Yellow

			$confirm = Read-Host "Kill this process? (y/n)"
			if ($confirm -eq 'y') {
				Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
				Write-Host "  ✓ Process killed" -ForegroundColor Green
			}
		} else {
			Write-Host "Port $port is free" -ForegroundColor Green
		}
	}

	Write-Host ""
}

# Main script logic
switch ($Action) {
	'start' {
		Show-Status
		Start-Services
	}
	'stop' {
		Stop-Services
		Show-Status
	}
	'status' {
		Show-Status
	}
	'cleanup' {
		Clean-Ports
		Show-Status
	}
}

# Show help
Write-Host "Usage: .\manage-services.ps1 [-Action <start|stop|status|cleanup>]" -ForegroundColor Gray
Write-Host ""
