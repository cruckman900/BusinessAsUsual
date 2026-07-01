# Stop all BusinessAsUsual services - NUCLEAR OPTION
# This script FORCE KILLS everything with extreme prejudice

# Check if running as admin
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "NOT RUNNING AS ADMIN - Attempting to elevate..." -ForegroundColor Red
    Start-Process powershell.exe -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`"" -Verb RunAs
    exit
}

Write-Host "KILLING ALL SERVICES - NO MERCY" -ForegroundColor Red
Write-Host ""

# NUCLEAR OPTION: Kill everything .NET related
Write-Host "KILLING ALL .NET PROCESSES..." -ForegroundColor Cyan
$processNames = @(
    "dotnet",
    "HR.Web",
    "BusinessAsUsual.Web",
    "BusinessAsUsual.API",
    "ModuleRegistry.API",
    "HR.API",
    "VBCSCompiler",
    "MSBuild",
    "devenv"
)

foreach ($name in $processNames) {
    $processes = Get-Process -Name $name -ErrorAction SilentlyContinue
    if ($processes) {
        Write-Host "  Killing $($processes.Count) instance(s) of $name..." -ForegroundColor Yellow
        $processes | Stop-Process -Force -ErrorAction SilentlyContinue
    }
}

Start-Sleep -Seconds 2
Write-Host "✓ ALL .NET PROCESSES TERMINATED" -ForegroundColor Green
Write-Host ""

# KILL EVERYTHING ON OUR PORTS
$ports = @(5100, 5001, 5002, 5269)

Write-Host "KILLING PROCESSES ON PORTS..." -ForegroundColor Cyan
foreach ($port in $ports) {
    $connections = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue

    if ($connections) {
        foreach ($connection in $connections) {
            $processId = $connection.OwningProcess
            if ($processId -gt 0) {
                $process = Get-Process -Id $processId -ErrorAction SilentlyContinue
                if ($process) {
                    Write-Host "  KILLING $($process.ProcessName) (PID: $processId) on port $port" -ForegroundColor Red
                    Stop-Process -Id $processId -Force -ErrorAction SilentlyContinue
                }
            }
        }
    } else {
        Write-Host "  Port $port is free" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "EVERYTHING IS DEAD!" -ForegroundColor Green
Write-Host "You can now run: .\start-all-services.ps1" -ForegroundColor Cyan
