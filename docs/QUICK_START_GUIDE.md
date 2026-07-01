# Quick Start Guide - Running Your Microservices

## ✅ Problem Fixed!

Your services can now run **without SQL Server** using in-memory databases. The database connection errors are resolved.

## Running Everything in Visual Studio (Easiest Method)

### Step 1: Configure Multiple Startup Projects

1. **In Solution Explorer**, right-click the **solution name** (at the top)
2. Select **Properties**
3. Go to **Common Properties** → **Startup Project**
4. Select **"Multiple startup projects"**
5. Set the **Action** to **"Start"** for these projects:
   - ✅ `ModuleRegistry.API` (under services/ModuleRegistry)
   - ✅ `HR.API` (under services/HR)
   - ✅ `HR.Web` (under services/HR) **← NEW! HR Web UI**
   - ✅ `BusinessAsUsual.Web` (under frontend)
6. Click **OK**

### Step 2: Start Everything

Press **F5** or click the **Start** button (green triangle)

All **FOUR** services will start simultaneously:
- **Module Registry**: http://localhost:5100
- **HR API**: http://localhost:5041
- **HR Web UI**: http://localhost:5002 **← NEW!**
- **Web Application**: Opens in your default browser

### Step 3: Verify It's Working

Check the **Output** window in Visual Studio (View → Output). You should see:

```
ModuleRegistry.API:
  ⚠️  Using in-memory database for development
  ✓ In-memory database ready
  Now listening on: http://localhost:5100

HR.API:
  ⚠️  Using in-memory database for HR Service
  ✓ In-memory database ready for HR Service
  ✓ Successfully registered HR module with Module Registry (including mobile support)
  Now listening on: http://localhost:5041

HR.Web:
  ⚠️  HR.Web using in-memory database
  ✓ Seeded sample HR data
  ✓ HR Web UI ready on http://localhost:5002

BusinessAsUsual.Web:
  ✓ Successfully discovered 1 modules from Module Registry
  Now listening on: http://localhost:5269
```

## What Each Service Does

### 📦 Module Registry Service (Port 5100)
- Central registry for all microservices
- Stores metadata about each module (web UI, mobile UI, APIs)
- Provides discovery endpoints for web/mobile apps
- **Endpoint**: http://localhost:5100/api/modules/ui

### 👥 HR API Service (Port 5041)
- Human Resources microservice backend
- Manages employees, departments, etc.
- REST API for data operations
- Automatically registers itself with Module Registry on startup
- Provides web UI components and mobile UI contracts
- **Swagger**: http://localhost:5041/swagger
- **Mobile UI Spec**: http://localhost:5041/api/hr/mobile/ui-spec

### 🌐 Web Application (Port 5269)
- Blazor Server web shell
- Dynamically discovers modules from Module Registry
- Loads HR module UI from the registry
- **URL**: http://localhost:5269

## Verifying Dynamic Module Injection

### How to Prove HR is Dynamically Loaded

The HR module you see in the web app is **discovered at runtime** from the Module Registry, not hardcoded.

**Test this**:
1. Look at the current HR module name in the web navigation
2. The Module Registry returns "Human Resources" as the display name
3. To prove it's dynamic, you would need to change the registry data and refresh

Since we're using an in-memory database, the data is reset on each restart. The HR service registers itself on startup with these details:

```csharp
// From HR.Application/Services/ModuleRegistrationService.cs
new RegisterModuleRequest
{
	ModuleId = "hr",
	DisplayName = "Human Resources",  // <-- Change this to test
	Description = "Manage employees, departments, onboarding, and benefits",
	// ... etc
}
```

## Testing the Module Registry API

### Get All Modules with UI
```powershell
Invoke-WebRequest -Uri http://localhost:5100/api/modules/ui -UseBasicParsing | 
  Select-Object -ExpandProperty Content | 
  ConvertFrom-Json | 
  ConvertTo-Json -Depth 5
```

### Get Modules with Mobile Support
```powershell
Invoke-WebRequest -Uri http://localhost:5100/api/modules/mobile -UseBasicParsing | 
  Select-Object -ExpandProperty Content | 
  ConvertFrom-Json | 
  ConvertTo-Json -Depth 5
```

### Get HR Mobile UI Specification
```powershell
Invoke-WebRequest -Uri http://localhost:5041/api/hr/mobile/ui-spec -UseBasicParsing | 
  Select-Object -ExpandProperty Content | 
  ConvertFrom-Json | 
  ConvertTo-Json -Depth 5
```

## Common Issues and Solutions

### ❌ "Port already in use" Error

**Problem**: Service fails to start because another instance is using the port.

**Solution**:
```powershell
# Find what's using the port (example for 5100)
Get-NetTCPConnection -LocalPort 5100 -ErrorAction SilentlyContinue

# Kill the process if needed
Stop-Process -Id <ProcessId> -Force
```

Or just restart Visual Studio - it will clean up background processes.

### ❌ "Cannot connect to Module Registry" in Web App

**Problem**: Web app shows errors about connecting to localhost:5100

**Solution**: Make sure Module Registry is running **before** the web app starts.
- In Visual Studio, check the startup order in project properties
- Or start Module Registry first, then the web app

### ❌ HR Module Not Appearing in Web UI

**Check**:
1. Is HR service running? (Check Output window)
2. Did HR register successfully? (Look for "✓ Successfully registered" message)
3. Is Module Registry running?
4. Check browser console (F12) for JavaScript errors
5. Check web app logs for Module Discovery errors

## Switching to SQL Server (When Ready)

When you want to use a real database instead of in-memory:

### For Module Registry
Edit: `services/ModuleRegistry/ModuleRegistry.API/appsettings.Development.json`
```json
{
  "UseInMemoryDatabase": false,  // <-- Change to false
  ...
}
```

Then set your connection string as an environment variable or in appsettings:
```
MRS_SQL_CONNECTION_STRING=Server=localhost;Database=BusinessAsUsual_ModuleRegistry;Trusted_Connection=True;TrustServerCertificate=True;
```

### For HR Service
Edit: `services/HR/HR.API/appsettings.Development.json`
```json
{
  "UseInMemoryDatabase": false,  // <-- Change to false
  ...
}
```

The `.env.local` file already has the HR connection string configured.

## Next Steps

Now that everything is running:

1. ✅ **Browse the web app** - See the HR module in navigation
2. ✅ **Test the Swagger UI** - http://localhost:5041/swagger
3. ✅ **Check mobile contracts** - http://localhost:5041/api/hr/mobile/ui-spec
4. ✅ **Add more modules** - Follow the same pattern as HR
5. ✅ **Customize HR display name** - Modify the registration to prove dynamic loading

## Related Documentation

- `docs/DATABASE_FREE_DEVELOPMENT.md` - Detailed explanation of in-memory database setup
- `docs/MICROSERVICEARCHITECTUREOVERVIEW.md` - System architecture
- `docs/MOBILE_UI_CONTRACT_IMPLEMENTATION.md` - Mobile UI contract specs
- `docs/MODULE_REGISTRY_SERVICE.md` - Module Registry details
- `docs/HANDOVER_DOCUMENT.md` - Original architecture vision

## Need Help?

The system is working! All services can now start without SQL Server. The Module Registry is successfully discovering the HR module, including its mobile UI contracts.
