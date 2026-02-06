# Powershell Commands

## ﻿🧭 Navigation & File System

### List files and folders
```powershell
Get-ChildItem
```

## Change directory
```powershell
Set-Location "C:\Path\To\Directory"
```

## Create a new folder
```powershell
New-Item -ItemType Directory -Path "NewFolder"
```

## Copy files or folders
```powershell
Copy-Item "source.txt" -Destination "C:\Backup\source.txt"
```

## Move files or folders
```powershell
Move-Item "old.txt" -Destination "C:\Archive\old.txt"
```

## Delete files or folders
```powershell
Remove-Item "unwanted.txt"
```
---

# 🔍 System Info & Diagnostics

## Get system info
Get-ComputerInfo

# Check memory usage
Get-CimInstance Win32_OperatingSystem | Select-Object FreePhysicalMemory, TotalVisibleMemorySize

# List running processes
Get-Process

# List services
Get-Service

# Check disk space
Get-PSDrive

📦 Package Management
# Install a module
Install-Module -Name ModuleName

# Update a module
Update-Module -Name ModuleName

# List installed modules
Get-InstalledModule

# Import a module
Import-Module ModuleName





🛠️ Scripting & Automation
# Define a function
function Say-Hello {
    param($Name)
    Write-Output "Hello, $Name!"
}

# Run a script
.\myscript.ps1

# Schedule a task
$action = New-ScheduledTaskAction -Execute "PowerShell.exe" -Argument "-File C:\Scripts\Backup.ps1"
$trigger = New-ScheduledTaskTrigger -Daily -At 9am
Register-ScheduledTask -Action $action -Trigger $trigger -TaskName "DailyBackup"





🔐 Security & Permissions
# Get ACL (permissions) of a file
Get-Acl "C:\Path\To\File.txt"

# Set ACL
$acl = Get-Acl "C:\Path\To\File.txt"
$rule = New-Object System.Security.AccessControl.FileSystemAccessRule("UserName","FullControl","Allow")
$acl.SetAccessRule($rule)
Set-Acl "C:\Path\To\File.txt" $acl



🌐 Networking
# Get IP configuration
Get-NetIPAddress

# Test network connection
Test-Connection google.com

# Get DNS settings
Get-DnsClientServerAddress

# Get active TCP connections
Get-NetTCPConnection





🧪 Environment & Variables
# List environment variables
Get-ChildItem Env:

# Set an environment variable
$env:MY_VAR = "HelloWorld"

# Persist environment variable (system-wide)
[Environment]::SetEnvironmentVariable("MY_VAR", "HelloWorld", "Machine")





🎯 Miscellaneous Power Moves
# Search for a string in files
Select-String -Path "*.log" -Pattern "ERROR"

# Export data to CSV
Get-Process | Export-Csv -Path "processes.csv" -NoTypeInformation

# Convert to JSON
Get-Service | ConvertTo-Json

🛠️ Scripting & Automation
# Define a function
function Say-Hello {
    param($Name)
    Write-Output "Hello, $Name!"
}

# Run a script
.\myscript.ps1

# Schedule a task
$action = New-ScheduledTaskAction -Execute "PowerShell.exe" -Argument "-File C:\Scripts\Backup.ps1"
$trigger = New-ScheduledTaskTrigger -Daily -At 9am
Register-ScheduledTask -Action $action -Trigger $trigger -TaskName "DailyBackup"


