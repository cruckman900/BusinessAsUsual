# Database-Free Local Development Setup

## Problem Fixed
The Module Registry Service and HR Service were failing to start because they couldn't connect to SQL Server. This prevented the web application from discovering modules.

## Solution Implemented
Both services now support **in-memory databases** for local development, eliminating the SQL Server dependency.

### Changes Made

#### 1. Module Registry Service (`services/ModuleRegistry/ModuleRegistry.API`)
- ✅ Added `Microsoft.EntityFrameworkCore.InMemory` package
- ✅ Modified `Program.cs` to detect `UseInMemoryDatabase` configuration setting
- ✅ Conditionally skip database health checks when using in-memory database
- ✅ Created `appsettings.Development.json` with `"UseInMemoryDatabase": true`

#### 2. HR Service (`services/HR/HR.API`)
- ✅ Added `Microsoft.EntityFrameworkCore.InMemory` package
- ✅ Modified `Program.cs` to detect `UseInMemoryDatabase` configuration setting
- ✅ Conditionally skip database health checks when using in-memory database
- ✅ Updated `appsettings.Development.json` with `"UseInMemoryDatabase": true`

## Running the Services

### Option 1: Visual Studio GUI (Recommended)

1. **Set Multiple Startup Projects**:
   - Right-click the solution → Properties → Common Properties → Startup Project
   - Select "Multiple startup projects"
   - Set these projects to **Start**:
	 - `ModuleRegistry.API` (services/ModuleRegistry/ModuleRegistry.API)
	 - `HR.API` (services/HR/HR.API)
	 - `BusinessAsUsual.Web` (frontend/BusinessAsUsual.Web)

2. **Press F5 or click Start**
   - All three services will start simultaneously
   - ModuleRegistry will listen on: `http://localhost:5100`
   - HR Service will listen on: `http://localhost:5041`
   - Web app will open in your browser automatically

### Option 2: Command Line

Start each service in a separate terminal window:

```powershell
# Terminal 1 - Module Registry
cd "D:\DotNet Projects\BusinessAsUsual\services\ModuleRegistry\ModuleRegistry.API"
dotnet run

# Terminal 2 - HR Service  
cd "D:\DotNet Projects\BusinessAsUsual\services\HR\HR.API"
dotnet run

# Terminal 3 - Web Application
cd "D:\DotNet Projects\BusinessAsUsual\frontend\BusinessAsUsual.Web"
dotnet run
```

## What You Should See

### Module Registry Service Startup
```
⚠️  Using in-memory database for development
✓ In-memory database ready
info: Now listening on: http://localhost:5100
```

### HR Service Startup
```
⚠️  Using in-memory database for HR Service
✓ In-memory database ready for HR Service
✓ Successfully registered HR module with Module Registry (including mobile support)
info: Now listening on: http://localhost:5041
```

### Web Application
- Should start without the previous connection refused error
- Module Discovery Service will successfully connect to `localhost:5100`
- HR module will appear in the navigation (dynamically loaded from MRS)

## Verification

### Test Module Registry Endpoint
```powershell
Invoke-WebRequest -Uri http://localhost:5100/api/modules/ui -UseBasicParsing
```

You should see HR module metadata including:
- `"moduleId": "hr"`
- `"displayName": "Human Resources"`
- `"supportsMobile": true`
- `"mobileUISpecUrl": "http://localhost:5001/api/hr/mobile/ui-spec"`

### Test HR Registration
The HR service automatically registers with the Module Registry on startup. Check the HR service console output for:
```
✓ Successfully registered HR module with Module Registry (including mobile support)
```

## Switching Back to SQL Server

To use SQL Server instead of in-memory database:

### For Module Registry
Edit `services/ModuleRegistry/ModuleRegistry.API/appsettings.Development.json`:
```json
{
  "UseInMemoryDatabase": false,
  ...
}
```

Or set an environment variable:
```powershell
$env:MRS_SQL_CONNECTION_STRING = "Server=localhost;Database=BusinessAsUsual_ModuleRegistry;Trusted_Connection=True;TrustServerCertificate=True;"
```

### For HR Service
Edit `services/HR/HR.API/appsettings.Development.json`:
```json
{
  "UseInMemoryDatabase": false,
  ...
}
```

Or use the `.env.local` file (already configured):
```
HR_SQL_CONNECTION_STRING=Server=localhost;Database=BusinessAsUsual_HR;Trusted_Connection=True;TrustServerCertificate=True;
```

## Benefits of In-Memory Database for Local Development

1. **No SQL Server Required**: Developers can work without installing/running SQL Server
2. **Fast Startup**: In-memory databases initialize instantly
3. **Clean State**: Each run starts with a fresh database
4. **CI/CD Friendly**: Automated tests can run without database infrastructure
5. **Easy Onboarding**: New developers can start immediately

## Limitations

- **Data is not persisted**: All data is lost when the service stops
- **Not for production**: Only enabled in Development environment
- **Limited SQL features**: Some SQL Server-specific features may not work

## Next Steps

- Start all three services using Visual Studio (recommended)
- Open the web application in your browser
- Verify that the HR module appears in the navigation
- Change the HR module's `DisplayName` in MRS to prove it's dynamically loaded
- Test the mobile UI contract endpoints at `http://localhost:5041/api/hr/mobile/ui-spec`
