# Platform Import Feature - Integration Complete

## Overview
Successfully integrated the reusable "best in class SQL import functionality" from Platform into the BusinessAsUsual.Web host application. The import wizard is now accessible via the Platform dashboard at `/platform/import`.

## Architecture

### Component Layers
- **Platform.Application**: Contracts and DTOs for import services
- **Platform.Infrastructure**: Implementation of import services (parsing, mapping, transformation, batch import)
- **Platform.Web**: Blazor UI components (Import wizard page, dashboard card, nav link)
- **BusinessAsUsual.Web**: Host application that registers all services at runtime

### Key Design Pattern
The Platform.Web library is a Razor Class Library (Sdk.Razor), not a standalone application. Therefore:
- UI components live in Platform.Web
- Service contracts live in Platform.Application
- Service implementations live in Platform.Infrastructure
- **Runtime DI registration happens in the host (BusinessAsUsual.Web)**

## Changes Made

### 1. Project References
**File**: `frontend/BusinessAsUsual.Web/BusinessAsUsual.Web.csproj`
- Added reference to `Platform.Infrastructure` to enable DI registration

### 2. Dependency Injection Setup
**File**: `frontend/BusinessAsUsual.Web/Program.cs`
- Added `using Platform.Infrastructure;` and `using HR.Infrastructure.Extensions;`
- Updated `RegisterPlatformModuleServices()` to:
  - Call `services.AddInfrastructure(configuration)` to register all Platform import services
  - Call `ImportServiceExtensions.RegisterHRTablesForImport()` to register HR tables for import
  - Register `ITenantContextProvider` adapter

### 3. Configuration
**File**: `frontend/BusinessAsUsual.Web/appsettings.json`
- Added `ConnectionStrings:PlatformDb` for Platform database operations

### 4. Tenant Context Abstraction
**New File**: `services/Platform/Platform.Application/Interfaces/ITenantContextProvider.cs`
- Created platform-agnostic interface for tenant context
- Provides `CompanyId`, `UserId`, and `IsResolved` properties

**New File**: `frontend/BusinessAsUsual.Web/Services/PlatformTenantContextAdapter.cs`
- Adapter that bridges `BusinessAsUsual.Application.Services.ITenantContext` to `Platform.Application.Interfaces.ITenantContextProvider`
- Registered in DI to make tenant context available to Platform components

### 5. Import Page Updates
**File**: `services/Platform/Platform.Web/Components/Pages/Import.razor`
- Injected `ITenantContextProvider` instead of direct tenant context
- Updated `StartImport()` to use real `CompanyId` and `UserId` from tenant context
- Removed placeholder GUID strings

## Access Points

### From Platform Dashboard
1. Navigate to `/platform` (Platform Home)
2. Click on "Import Data" card in the Platform modules grid
3. Or use the "Import Data" link in the Platform sidebar navigation

### Direct URL
- `/platform/import`

## Services Registered

The following services are now available for dependency injection in the host app:

1. **ISchemaIntrospectionService**: Database schema discovery and table metadata
2. **IFileParserService**: CSV and Excel file parsing
3. **IDataTransformationService**: Data transformation and cleaning
4. **IColumnMappingService**: Intelligent column mapping and validation
5. **IBatchImportService**: Batch data import with progress tracking
6. **ITenantContextProvider**: Tenant and user context for multi-tenant operations

## Tables Available for Import

Currently registered tables (HR module):
- **Employees**: Employee master data
- **Departments**: Department hierarchy

Additional tables can be registered by calling the appropriate `Register*TablesForImport()` extension methods in the module startup.

## Feature Status

### ✅ Completed
- [x] Reusable import infrastructure in Platform.Application/Infrastructure
- [x] Import wizard UI moved to Platform.Web
- [x] Dashboard and navigation entry points
- [x] Runtime DI registration in host app
- [x] Real tenant/user context integration
- [x] Build verification successful

### 🚧 Remaining Work
- [ ] Implement actual SQL Server bulk insert (currently simulated)
- [ ] Build transformation editor dialog (currently shows "coming soon")
- [ ] Add preview of transformed data before import
- [ ] Create import history & rollback UI
- [ ] Add save/load column mapping templates
- [ ] Create unit tests for import services
- [ ] Add more module tables to the import registry

## Testing Next Steps

1. Run the BusinessAsUsual.Web application
2. Navigate to `/platform/import`
3. Verify the page loads without errors
4. Test the import wizard with sample CSV/Excel files
5. Verify tenant context is correctly captured during import

## Notes

- The import wizard uses MudBlazor components for consistent UI
- File parsing supports both CSV and Excel (.xlsx) formats
- Column mapping includes intelligent fuzzy matching
- Progress tracking with cancellation support
- All import operations are logged with tenant/user context for audit trails
