# =============================================================================
# set-alb-url.ps1
# -----------------------------------------------------------------------------
# Patches the Android app's AWS endpoint placeholders with the live ALB DNS
# produced by 02-provision-infra.ps1 (.outputs.json -> albDns).
#
# Replaces every occurrence of "REPLACE_WITH_ALB_DNS" in data/build.gradle.kts.
# Safe to re-run: it also matches a previously-patched host, so you can point
# the app at a new ALB without hand-editing gradle.
#
# Usage:
#   ./set-alb-url.ps1
#   ./set-alb-url.ps1 -AlbDns my-alb-123.us-east-1.elb.amazonaws.com
#   ./set-alb-url.ps1 -GradlePath "D:\Android Projects\BusinessAsUsual_Android\data\build.gradle.kts"
# =============================================================================
param(
	[string]$AlbDns,
	[string]$GradlePath = "D:\Android Projects\BusinessAsUsual_Android\data\build.gradle.kts"
)

$ErrorActionPreference = "Stop"

# --- Resolve the ALB DNS (arg wins, else read .outputs.json) ---
if (-not $AlbDns) {
	$outputsPath = Join-Path $PSScriptRoot ".outputs.json"
	if (-not (Test-Path $outputsPath)) {
		throw "No -AlbDns given and '$outputsPath' not found. Run 02-provision-infra.ps1 first, or pass -AlbDns."
	}
	$AlbDns = (Get-Content $outputsPath -Raw | ConvertFrom-Json).albDns
}
if ([string]::IsNullOrWhiteSpace($AlbDns)) {
	throw "ALB DNS is empty. Check .outputs.json or pass -AlbDns."
}

# Normalize: strip scheme / trailing slash if the caller pasted a full URL.
$AlbDns = $AlbDns -replace '^https?://', '' -replace '/+$', ''

if (-not (Test-Path $GradlePath)) {
	throw "Gradle file not found: $GradlePath"
}

# --- Patch the AWS_* buildConfigField host values ---
$content = Get-Content $GradlePath -Raw

# Rewrite the host inside AWS_HR_URL / AWS_REGISTRY_URL regardless of current value
# (placeholder or an old ALB DNS), preserving the http scheme + trailing slash.
$pattern = 'buildConfigField\("String",\s*"(AWS_HR_URL|AWS_REGISTRY_URL)",\s*"\\"http://[^"\\]+/\\""\)'
$patched = [System.Text.RegularExpressions.Regex]::Replace(
	$content,
	$pattern,
	{ param($m) 'buildConfigField("String", "' + $m.Groups[1].Value + '", "\"http://' + $AlbDns + '/\"")' }
)

if ($patched -eq $content) {
	Write-Host "No AWS_HR_URL / AWS_REGISTRY_URL fields matched. Nothing changed." -ForegroundColor Yellow
	Write-Host "Verify the buildConfigField lines in: $GradlePath" -ForegroundColor Yellow
	return
}

Set-Content -Path $GradlePath -Value $patched -Encoding UTF8
Write-Host "Patched AWS endpoints -> http://$AlbDns/" -ForegroundColor Green
Write-Host "File: $GradlePath" -ForegroundColor DarkGray
Write-Host "Now rebuild the app:  ./gradlew :app:assembleRelease" -ForegroundColor Cyan
