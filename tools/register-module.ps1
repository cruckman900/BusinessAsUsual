param(
	[string]$ModuleFile = "services/Services/ModuleRegistration.json",
	[string]$RegistryUrl = "http://localhost:5100/api/modules"
)

if (-not (Test-Path $ModuleFile)) {
	Write-Error "Module registration file not found: $ModuleFile"
	exit 1
}

$json = Get-Content $ModuleFile -Raw

try {
	$resp = Invoke-RestMethod -Uri $RegistryUrl -Method Post -Body $json -ContentType 'application/json'
	Write-Host "Module registration response:" -ForegroundColor Green
	$resp | ConvertTo-Json -Depth 5 | Write-Host
}
catch {
	Write-Error "Failed to register module: $_"
}
