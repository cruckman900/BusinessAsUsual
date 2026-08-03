# BusinessAsUsual - Test Automation Script
# This script runs all tests and generates a comprehensive coverage report

param(
	[Parameter(Mandatory=$false)]
	[string]$Module = "All",

	[Parameter(Mandatory=$false)]
	[switch]$Coverage,

	[Parameter(Mandatory=$false)]
	[switch]$VerboseOutput
)

$ErrorActionPreference = "Continue"
$RootPath = "D:\DotNet Projects\BusinessAsUsual"

# Test projects configuration
$TestProjects = @(
	@{ Name = "Sales.Tests"; Path = "services\Sales\Sales.Tests\Sales.Tests.csproj"; Type = "Unit" }
	@{ Name = "Sales.IntegrationTests"; Path = "services\Sales\Sales.IntegrationTests\Sales.IntegrationTests.csproj"; Type = "Integration" }
	@{ Name = "Finance.Tests"; Path = "services\Finance\Finance.Tests\Finance.Tests.csproj"; Type = "Unit" }
	@{ Name = "CRM.Tests"; Path = "services\CRM\CRM.Tests\CRM.Tests.csproj"; Type = "Unit" }
	@{ Name = "HR.Tests"; Path = "services\HR\HR.Tests\HR.Tests.csproj"; Type = "Unit" }
	@{ Name = "Inventory.Tests"; Path = "services\Inventory\Inventory.Tests\Inventory.Tests.csproj"; Type = "Unit" }
	@{ Name = "ModuleRegistry.Tests"; Path = "services\ModuleRegistry\ModuleRegistry.Tests\ModuleRegistry.Tests.csproj"; Type = "Unit" }
	@{ Name = "AI.Tests"; Path = "services\AI\AI.Tests\AI.Tests.csproj"; Type = "Unit" }
	@{ Name = "BusinessAsUsual.Tests"; Path = "backend\BusinessAsUsual.Tests\BusinessAsUsual.Tests.csproj"; Type = "Unit" }
)

# Results tracking
$Results = @()
$TotalTests = 0
$TotalPassed = 0
$TotalFailed = 0
$TotalSkipped = 0

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "BusinessAsUsual Test Automation Suite" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Filter projects by module if specified
if ($Module -ne "All") {
	$TestProjects = $TestProjects | Where-Object { $_.Name -like "$Module*" }
	if ($TestProjects.Count -eq 0) {
		Write-Host "No test projects found for module: $Module" -ForegroundColor Red
		exit 1
	}
}

# Run each test project
foreach ($project in $TestProjects) {
	$projectPath = Join-Path $RootPath $project.Path

	if (-not (Test-Path $projectPath)) {
		Write-Host "⚠️  Project not found: $($project.Name)" -ForegroundColor Yellow
		continue
	}

	Write-Host "Running: $($project.Name) [$($project.Type)]" -ForegroundColor Cyan
	Write-Host "Path: $projectPath" -ForegroundColor Gray
	Write-Host ""

	$loggerArgs = if ($VerboseOutput) { "console;verbosity=detailed" } else { "console;verbosity=minimal" }
	$coverageArgs = if ($Coverage) { "--collect:`"XPlat Code Coverage`"" } else { "" }

	$testCommand = "dotnet test `"$projectPath`" --logger `"$loggerArgs`" $coverageArgs --no-build"

	try {
		$output = Invoke-Expression $testCommand 2>&1
		$exitCode = $LASTEXITCODE

		# Parse test results from output
		$passed = 0
		$failed = 0
		$skipped = 0
		$total = 0

		if ($output -match "Passed:\s+(\d+)") { $passed = [int]$Matches[1] }
		if ($output -match "Failed:\s+(\d+)") { $failed = [int]$Matches[1] }
		if ($output -match "Skipped:\s+(\d+)") { $skipped = [int]$Matches[1] }
		if ($output -match "Total:\s+(\d+)") { $total = [int]$Matches[1] }

		$status = if ($failed -eq 0) { "✅ PASSED" } else { "❌ FAILED" }

		$Results += [PSCustomObject]@{
			Project = $project.Name
			Type = $project.Type
			Status = $status
			Total = $total
			Passed = $passed
			Failed = $failed
			Skipped = $skipped
		}

		$TotalTests += $total
		$TotalPassed += $passed
		$TotalFailed += $failed
		$TotalSkipped += $skipped

		if ($failed -gt 0) {
			Write-Host "$status - $total tests: $passed passed, $failed failed, $skipped skipped" -ForegroundColor Red
		} else {
			Write-Host "$status - $total tests: $passed passed" -ForegroundColor Green
		}

	} catch {
		Write-Host "❌ ERROR running tests: $_" -ForegroundColor Red
		$Results += [PSCustomObject]@{
			Project = $project.Name
			Type = $project.Type
			Status = "❌ ERROR"
			Total = 0
			Passed = 0
			Failed = 0
			Skipped = 0
		}
	}

	Write-Host ""
}

# Generate summary report
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "TEST SUMMARY" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$Results | Format-Table -AutoSize

Write-Host ""
Write-Host "Total Statistics:" -ForegroundColor Cyan
Write-Host "  Total Tests: $TotalTests" -ForegroundColor White
Write-Host "  Passed: $TotalPassed" -ForegroundColor Green
Write-Host "  Failed: $TotalFailed" -ForegroundColor $(if ($TotalFailed -gt 0) { "Red" } else { "White" })
Write-Host "  Skipped: $TotalSkipped" -ForegroundColor Yellow

$successRate = if ($TotalTests -gt 0) { [math]::Round(($TotalPassed / $TotalTests) * 100, 2) } else { 0 }
Write-Host "  Success Rate: $successRate%" -ForegroundColor $(if ($successRate -ge 90) { "Green" } elseif ($successRate -ge 70) { "Yellow" } else { "Red" })

Write-Host ""

# Coverage report generation
if ($Coverage) {
	Write-Host "========================================" -ForegroundColor Cyan
	Write-Host "CODE COVERAGE REPORT" -ForegroundColor Cyan
	Write-Host "========================================" -ForegroundColor Cyan
	Write-Host ""

	# Check if reportgenerator is installed
	$reportGeneratorExists = Get-Command reportgenerator -ErrorAction SilentlyContinue

	if ($reportGeneratorExists) {
		Write-Host "Generating HTML coverage report..." -ForegroundColor Cyan

		$coverageFiles = Get-ChildItem -Path $RootPath -Recurse -Filter "coverage.cobertura.xml" | Select-Object -ExpandProperty FullName

		if ($coverageFiles.Count -gt 0) {
			$reportPath = Join-Path $RootPath "coverage-report"
			$reports = ($coverageFiles -join ";")

			reportgenerator -reports:"$reports" -targetdir:"$reportPath" -reporttypes:Html

			Write-Host "✅ Coverage report generated at: $reportPath\index.html" -ForegroundColor Green

			# Open report in browser
			$indexPath = Join-Path $reportPath "index.html"
			if (Test-Path $indexPath) {
				Start-Process $indexPath
			}
		} else {
			Write-Host "⚠️  No coverage files found. Make sure tests ran with --collect option." -ForegroundColor Yellow
		}
	} else {
		Write-Host "⚠️  reportgenerator tool not installed." -ForegroundColor Yellow
		Write-Host "Install with: dotnet tool install -g dotnet-reportgenerator-globaltool" -ForegroundColor Gray
	}
}

# Exit with appropriate code
if ($TotalFailed -gt 0) {
	Write-Host "⚠️  Some tests failed!" -ForegroundColor Red
	exit 1
} else {
	Write-Host "✅ All tests passed!" -ForegroundColor Green
	exit 0
}
