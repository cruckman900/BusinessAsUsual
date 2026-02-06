# Powershell Commands

## ﻿🧭 Navigation & File System

### List files and folders
```powershell
Get-ChildItem
```

### Change directory
```powershell
Set-Location "C:\Path\To\Directory"
```

### Create a new folder
```powershell
New-Item -ItemType Directory -Path "NewFolder"
```

### Copy files or folders
```powershell
Copy-Item "source.txt" -Destination "C:\Backup\source.txt"
```

### Move files or folders
```powershell
Move-Item "old.txt" -Destination "C:\Archive\old.txt"
```

### Delete files or folders
```powershell
Remove-Item "unwanted.txt"
```

---

## 🔍 System Info & Diagnostics

### Get system info
```powershell
Get-ComputerInfo
```

### Check memory usage
```powershell
Get-CimInstance Win32_OperatingSystem | Select-Object FreePhysicalMemory, TotalVisibleMemorySize
```

### List running processes
```powershell
Get-Process
```

### List services
```powershell
Get-Service
```

### Check disk space
```powershell
Get-PSDrive
```

---

## 📦 Package Management

### Install a module
```powershell
Install-Module -Name ModuleName
```

### Update a module
```powershell
Update-Module -Name ModuleName
```

### List installed modules
```powershell
Get-InstalledModule
```

### Import a module
```powershell
Import-Module ModuleName
```

---

## 🛠️ Scripting & Automation

### Define a function
```powershell
function Say-Hello {
    param($Name)
    Write-Output "Hello, $Name!"
}
```

### Run a script
```powershell
.\myscript.ps1
```

### Schedule a task
```powershell
$action = New-ScheduledTaskAction -Execute "PowerShell.exe" -Argument "-File C:\Scripts\Backup.ps1"
$trigger = New-ScheduledTaskTrigger -Daily -At 9am
Register-ScheduledTask -Action $action -Trigger $trigger -TaskName "DailyBackup"
```

---

## 🔐 Security & Permissions

### Get ACL (permissions) of a file
```powershell
Get-Acl "C:\Path\To\File.txt"
```

### Set ACL
```powershell
$acl = Get-Acl "C:\Path\To\File.txt"
$rule = New-Object System.Security.AccessControl.FileSystemAccessRule("UserName","FullControl","Allow")
$acl.SetAccessRule($rule)
Set-Acl "C:\Path\To\File.txt" $acl
```

---

## 🌐 Networking

### Get IP configuration
```powershell
Get-NetIPAddress
```

### Test network connection
```powershell
Test-Connection google.com
```

### Get DNS settings
```powershell
Get-DnsClientServerAddress
```

### Get active TCP connections
```powershell
Get-NetTCPConnection
```

---

## 🧪 Environment & Variables

### List environment variables
```powershell
Get-ChildItem Env:
```

### Set an environment variable
```powershell
$env:MY_VAR = "HelloWorld"
```

### Persist environment variable (system-wide)
```powershell
[Environment]::SetEnvironmentVariable("MY_VAR", "HelloWorld", "Machine")
```

---

## 🎯 Miscellaneous Power Moves

### Search for a string in files
```powershell
Select-String -Path "*.log" -Pattern "ERROR"
```

### Export data to CSV
```powershell
Get-Process | Export-Csv -Path "processes.csv" -NoTypeInformation
```

### Convert to JSON
```powershell
Get-Service | ConvertTo-Json
```

---

## 🛠️ Scripting & Automation

### Define a function
```powershell
function Say-Hello {
    param($Name)
    Write-Output "Hello, $Name!"
}
```

### Run a script
```powershell
.\myscript.ps1
```

### Schedule a task
```powershell
$action = New-ScheduledTaskAction -Execute "PowerShell.exe" -Argument "-File C:\Scripts\Backup.ps1"
$trigger = New-ScheduledTaskTrigger -Daily -At 9am
Register-ScheduledTask -Action $action -Trigger $trigger -TaskName "DailyBackup"
```
