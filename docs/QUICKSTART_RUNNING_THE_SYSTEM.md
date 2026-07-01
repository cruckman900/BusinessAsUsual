# 🚀 Quick Start Guide - Running the UI Injection System

## Prerequisites

Before starting, make sure you have:
- ✅ SQL Server running (LocalDB or full SQL Server)
- ✅ .NET 9 SDK installed
- ✅ All projects built successfully

## 🔧 Setup: SQL Server

### Option 1: Using SQL Server LocalDB (Easiest)

Update connection strings to use LocalDB:

**Module Registry (`services/ModuleRegistry/ModuleRegistry.API/Program.cs`):**
```csharp
var connectionString = Environment.GetEnvironmentVariable("MRS_SQL_CONNECTION_STRING")
    ?? "Server=(localdb)\\mssqllocaldb;Database=BusinessAsUsual_ModuleRegistry;Trusted_Connection=True;TrustServerCertificate=True;";
```

**HR Service (`services/HR/HR.Infrastructure/ConfigLoader.cs` or set environment variable):**
```
HR_SQL_CONNECTION_STRING=Server=(localdb)\\mssqllocaldb;Database=BusinessAsUsual_HR;Trusted_Connection=True;TrustServerCertificate=True;
```

### Option 2: Skip Database (For Testing UI Only)

The services will start even if SQL Server isn't available - they'll just log warnings. This is fine for testing the UI injection without data.

## 📋 Step-by-Step Instructions

### Method 1: Using PowerShell (3 Terminals)

#### Terminal 1: Module Registry Service

```powershell
cd "D:\DotNet Projects\BusinessAsUsual\services\ModuleRegistry\ModuleRegistry.API"
dotnet build
dotnet run
```

**Expected Output:**
```
Checking database connection...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5100
```

**Keep this terminal open!**

#### Terminal 2: HR Service

Open a NEW PowerShell window:

```powershell
cd "D:\DotNet Projects\BusinessAsUsual\services\HR\HR.API"
dotnet build
dotnet run
```

**Expected Output:**
```
Checking HR database connection...
✓ Successfully registered HR module with Module Registry (including mobile support)
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5001
```

**Keep this terminal open!**

#### Terminal 3: Web Application

Open a NEW PowerShell window:

```powershell
cd "D:\DotNet Projects\BusinessAsUsual\frontend\BusinessAsUsual.Web"
dotnet build
dotnet run
```

**Expected Output:**
```
info: BusinessAsUsual.Web.Services.ModuleDiscoveryService[0]
      Successfully discovered 1 modules from Module Registry
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5000
```

**Keep this terminal open!**

### Method 2: Using Visual Studio (Recommended!)

Much easier - everything starts together:

1. **Open Visual Studio**
2. **Right-click Solution** in Solution Explorer
3. **Click "Configure Startup Projects..."**
4. **Select "Multiple startup projects"**
5. **Set Action to "Start" for:**
   - `ModuleRegistry.API`
   - `HR.API`
   - `BusinessAsUsual.Web`
6. **Click OK**
7. **Press F5** (or click Start)

All three services will start automatically!

## 🌐 Open in Browser

Once all services are running, open:
```
https://localhost:5000
```

## ✅ What You Should See

### 1. Top Navigation
- The **HR** module should appear dynamically in the top navigation bar
- It should show "Human Resources" or "HR" depending on screen size

### 2. Click on HR
- Clicking should navigate you to `/hr`
- You should see the HR landing page with cards for:
  - Onboard Employee
  - Employee Directory  
  - Benefits

### 3. Browser Console (F12)
Press F12 and check the Console tab:
```
Successfully discovered modules from Module Registry
```

## 🧪 Testing the System

### Test 1: Verify Module Registry

Open in browser:
```
http://localhost:5100/swagger
```

Try the `GET /api/modules` endpoint. You should see:
```json
[
  {
    "moduleId": "hr",
    "displayName": "Human Resources",
    "supportsMobile": true,
    "mobileUISpecUrl": "http://localhost:5001/api/hr/mobile/ui-spec",
    "isActive": true
  }
]
```

### Test 2: Verify HR Mobile Contracts

Open in browser:
```
http://localhost:5001/swagger
```

Try the `GET /api/hr/mobile/ui-spec` endpoint to see the mobile UI specification.

### Test 3: Test Fallback Mechanism

1. Stop the Module Registry Service (Ctrl+C in Terminal 1)
2. Refresh the web browser
3. You should still see modules (from fallback list)
4. Console should show: "Using fallback module list"

## 🐛 Troubleshooting

### Issue: "Cannot connect to database"

**Solution:** This is OK! The services will start anyway. You just won't have data.

To fix it permanently:
- Install SQL Server LocalDB
- Or update connection strings to point to your SQL Server instance

### Issue: Port already in use

**Error:** "Address already in use"

**Solution:**
1. Find the process using the port:
   ```powershell
   netstat -ano | findstr :5100
   ```
2. Kill it:
   ```powershell
   taskkill /PID <process_id> /F
   ```

Or change ports in `launchSettings.json` files.

### Issue: Module Registry won't start

**Error:** Build errors or locked files

**Solution:**
```powershell
# Stop all running processes
Stop-Process -Name "ModuleRegistry.API" -Force -ErrorAction SilentlyContinue
Stop-Process -Name "HR.API" -Force -ErrorAction SilentlyContinue

# Clean and rebuild
cd "D:\DotNet Projects\BusinessAsUsual"
dotnet clean
dotnet build
```

### Issue: HR module not showing in web app

**Check:**
1. Is MRS running? (Check http://localhost:5100/health)
2. Is HR service running? (Check http://localhost:5001/health)
3. Did HR register? (Check HR console for "Successfully registered")
4. Check Web console logs (F12) for errors

### Issue: "No modules found"

**This means:**
- Module Registry Service isn't running
- Or Web app can't reach it

**Solution:**
1. Verify MRS is running: `http://localhost:5100/health`
2. Check `appsettings.json` in Web app has correct URL
3. Try restarting all services

## 🎯 Success Checklist

- [ ] Module Registry Service started (port 5100)
- [ ] HR Service started (port 5001)
- [ ] Web Application started (port 5000)
- [ ] HR console shows "Successfully registered"
- [ ] Web console shows "Successfully discovered modules"
- [ ] Browser shows HR in navigation
- [ ] Clicking HR navigates to `/hr`
- [ ] Swagger endpoints work for MRS and HR

## 📱 Testing Mobile Contracts

Use the `.http` files in VS Code or VS 2026:

**File:** `services/HR/HR.API/HR.Mobile.http`

```http
### Get mobile modules
GET http://localhost:5100/api/modules/mobile

### Get HR UI spec
GET http://localhost:5001/api/hr/mobile/ui-spec

### Get employee list spec
GET http://localhost:5001/api/hr/mobile/ui-spec/employee-list
```

## 🎉 Next Steps

Once you see it working:

1. **Explore Swagger UIs**
   - MRS: http://localhost:5100/swagger
   - HR: http://localhost:5001/swagger

2. **Check Mobile Specifications**
   - Browse the JSON responses
   - See how screens are defined

3. **Add More Modules**
   - Copy HR service structure
   - Create Accounting, Inventory, etc.
   - Watch them appear automatically!

4. **Test Without Database**
   - Stop SQL Server
   - Services still work (with warnings)
   - Great for UI-only development

## 🛑 Stopping Everything

**PowerShell Method:**
- Press `Ctrl+C` in each terminal

**Visual Studio Method:**
- Click Stop (red square) or press Shift+F5

**Force Stop All:**
```powershell
Stop-Process -Name "ModuleRegistry.API" -Force -ErrorAction SilentlyContinue
Stop-Process -Name "HR.API" -Force -ErrorAction SilentlyContinue
Stop-Process -Name "BusinessAsUsual.Web" -Force -ErrorAction SilentlyContinue
```

## 📚 Documentation

- [Module Registry & UI Injection](../docs/MODULE_REGISTRY_AND_UI_INJECTION.md)
- [Mobile UI Contract System](../docs/MOBILE_UI_CONTRACT_SYSTEM.md)
- [Complete Implementation](../docs/MOBILE_UI_CONTRACT_COMPLETE.md)

---

**Happy Testing! 🚀**

The system is designed to work even without SQL Server, so don't let database issues stop you from seeing the UI injection in action!
