# Quick Start: Module Registry & UI Injection

## Overview

This guide will help you quickly test the new Module Registry Service and UI injection functionality.

## Prerequisites

- .NET 9 SDK installed
- SQL Server running locally
- Visual Studio 2026 or VS Code

## Step-by-Step Guide

### 1. Start Module Registry Service

Open a terminal and run:

```bash
cd "D:\DotNet Projects\BusinessAsUsual\services\ModuleRegistry\ModuleRegistry.API"
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5100
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7100
```

**Test it:**
- Open browser: `http://localhost:5100/swagger`
- You should see Swagger UI with the Module Registry API

### 2. Start HR Service

Open a new terminal and run:

```bash
cd "D:\DotNet Projects\BusinessAsUsual\services\HR\HR.API"
dotnet run
```

**Expected Output:**
```
Successfully registered with Module Registry Service
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5001
```

**Test it:**
- Open browser: `http://localhost:5100/api/modules`
- You should see the HR module in the list:

```json
[
  {
    "moduleId": "hr",
    "displayName": "Human Resources",
    "description": "Manage employees, departments, onboarding, and benefits",
    "version": "1.0.0",
    "apiBaseUrl": "http://localhost:5001",
    "uiEntryPoint": "http://localhost:5002/hr",
    "icon": "mdi-account-group",
    "isActive": true,
    "healthStatus": "Unknown"
  }
]
```

### 3. Start Web Application

Open a new terminal and run:

```bash
cd "D:\DotNet Projects\BusinessAsUsual\frontend\BusinessAsUsual.Web"
dotnet run
```

**Expected Output:**
```
info: BusinessAsUsual.Web.Services.ModuleDiscoveryService[0]
      Successfully discovered 1 modules from Module Registry
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5000
```

**Test it:**
- Open browser: `https://localhost:5000`
- You should see the HR module in the top navigation
- Click on "HR" to navigate to the HR module

### 4. Test Fallback Mechanism

This tests what happens when Module Registry Service is unavailable.

1. **Stop the Module Registry Service** (Ctrl+C in the MRS terminal)
2. **Restart the Web Application** (Ctrl+C and then `dotnet run`)

**Expected Output:**
```
warn: BusinessAsUsual.Web.Services.ModuleDiscoveryService[0]
      Failed to fetch modules from Module Registry. Status: ServiceUnavailable
info: BusinessAsUsual.Web.Services.ModuleDiscoveryService[0]
      Using fallback module list
```

**Test it:**
- Open browser: `https://localhost:5000`
- You should still see modules (HR, Finance, CRM) from the fallback list
- Navigation should still work

## Verification Checklist

Use this checklist to verify everything is working:

- [ ] Module Registry Service starts without errors
- [ ] Module Registry Swagger UI is accessible
- [ ] HR Service starts and registers with MRS
- [ ] HR module appears in `/api/modules` endpoint
- [ ] Web Application starts without errors
- [ ] Web app logs show "Successfully discovered X modules"
- [ ] HR module appears in the web app navigation
- [ ] Clicking HR module navigates to `/hr` route
- [ ] Stopping MRS triggers fallback mechanism
- [ ] Fallback modules appear in navigation

## Troubleshooting

### Module Registry Service won't start

**Error:** `Unable to connect to SQL Server`

**Solution:** Check your SQL Server connection string:
- Set environment variable: `MRS_SQL_CONNECTION_STRING`
- Or update connection string in `Program.cs`

### HR Service won't register

**Error:** `Failed to register with Module Registry`

**Solution:**
- Verify MRS is running on port 5100
- Check `appsettings.json` in HR.API:
  ```json
  {
    "ModuleRegistry": {
      "Url": "http://localhost:5100"
    }
  }
  ```

### Web App doesn't show modules

**Error:** `Failed to fetch modules from Module Registry`

**Solution:**
- Verify MRS is running on port 5100
- Check browser console for errors
- Check `appsettings.json` in BusinessAsUsual.Web:
  ```json
  {
    "ModuleRegistry": {
      "Url": "http://localhost:5100"
    }
  }
  ```
- If MRS is down, fallback modules should still appear

### Port already in use

**Error:** `Address already in use`

**Solution:**
- Change ports in `launchSettings.json`
- Update port references in `appsettings.json` files

## Next Steps

### Add Another Module

Try creating a new microservice module:

1. Copy the HR service structure
2. Rename to your module (e.g., "Finance", "Inventory")
3. Update module registration metadata
4. Update port numbers
5. Start the service
6. Watch it automatically appear in the Web app!

### Enable Additional Features

- Add health checking with background worker
- Implement module versioning
- Add authentication for module registration
- Create module management UI in Admin app

## Visual Studio Debugging

To debug all services simultaneously:

1. **Set Multiple Startup Projects:**
   - Right-click solution → Properties
   - Select "Multiple startup projects"
   - Set these to "Start":
     - ModuleRegistry.API
     - HR.API
     - BusinessAsUsual.Web

2. **Press F5** to start all services

3. **Set breakpoints** in:
   - `ModuleRegistryService.RegisterModuleAsync()` (MRS)
   - `ModuleRegistrationService.RegisterWithModuleRegistryAsync()` (HR)
   - `ModuleDiscoveryService.GetModulesWithUiAsync()` (Web)

## Summary

You now have:
✅ A working Module Registry Service  
✅ HR Service self-registering  
✅ Web app dynamically loading modules  
✅ Fallback mechanism for resilience  

The architecture is ready for more modules to be added! 🚀

## Questions or Issues?

- Check the [Implementation Summary](./UI_INJECTION_IMPLEMENTATION_SUMMARY.md)
- Review the [Architecture Documentation](./MICROSERVICEARCHITECTUREOVERVIEW.md)
- Look at the [Module Registry Guide](./MODULE_REGISTRY_AND_UI_INJECTION.md)
