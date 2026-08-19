# Multi-Tenancy & Provisioning Platform Enhancement Plan

## 🎯 **Project Goals**

1. **Build out actual SQL provisioning scripts** for core tables and module-specific schemas
2. **Wire up multi-tenancy across all modules** with `CompanyId` (GUID)
3. **Enhance module/submodule selection** UI with granular control
4. **Create distributed provisioning system** where each microservice hosts its own schema scripts
5. **Migrate from string-based to JSON-based** module/submodule storage
6. **Mirror provisioning UX** from Admin to customer-facing Web signup

---

## 📋 **Phase 1: Core Schema & Multi-Tenancy Foundation**

### 1.1 Enhance DefaultSchema.sql (Base Tables)

**Goal:** Create comprehensive base schema with ALL necessary columns and proper multi-tenancy support.

**Core Tables to Add/Enhance:**

- [x] `CompanyInfo` - Already exists
- [x] `CompanySettings` - Already exists
- [x] `CompanyLocation` - Already exists
- [x] `BillingHistory` - Already exists
- [x] `ModuleUsage` - Already exists
- [x] `AuditLog` - Already exists
- [ ] **NEW: `Users`** - Company admin and employee users
  - `Id UNIQUEIDENTIFIER PRIMARY KEY`
  - `CompanyId UNIQUEIDENTIFIER NOT NULL`
  - `Email NVARCHAR(255) NOT NULL`
  - `PasswordHash NVARCHAR(MAX)` (or use external auth)
  - `FirstName NVARCHAR(100)`
  - `LastName NVARCHAR(100)`
  - `Role NVARCHAR(50)` (Admin, User, etc.)
  - `IsActive BIT DEFAULT 1`
  - `CreatedAt DATETIME DEFAULT GETUTCDATE()`
  - `LastLoginAt DATETIME`
- [ ] **NEW: `Roles`** - Role definitions
  - `Id UNIQUEIDENTIFIER PRIMARY KEY`
  - `CompanyId UNIQUEIDENTIFIER NOT NULL`
  - `RoleName NVARCHAR(100) NOT NULL`
  - `Permissions NVARCHAR(MAX)` (JSON array of permission strings)
- [ ] **NEW: `ModuleRegistry`** - Tracks enabled modules/submodules per company
  - `Id UNIQUEIDENTIFIER PRIMARY KEY`
  - `CompanyId UNIQUEIDENTIFIER NOT NULL`
  - `ModuleConfiguration NVARCHAR(MAX)` (JSON: see format below)
  - `UpdatedAt DATETIME DEFAULT GETUTCDATE()`
- [ ] **NEW: `ApiKeys`** - For tenant API access
  - `Id UNIQUEIDENTIFIER PRIMARY KEY`
  - `CompanyId UNIQUEIDENTIFIER NOT NULL`
  - `KeyName NVARCHAR(100)`
  - `KeyHash NVARCHAR(MAX)`
  - `CreatedAt DATETIME`
  - `ExpiresAt DATETIME`
  - `LastUsedAt DATETIME`
- [ ] **Enhance: `Employees`** - Add missing columns
  - Add `DateOfBirth DATE`
  - Add `PhoneNumber NVARCHAR(25)`
  - Add `Department NVARCHAR(100)`
  - Add `JobTitle NVARCHAR(100)`
  - Add `HireDate DATE`
  - Add `EmploymentType NVARCHAR(50)` (FullTime, PartTime, Contract)
  - Add `Status NVARCHAR(50)` (Active, OnLeave, Terminated)

**Actions:**
- [ ] Update `backend/BusinessAsUsual.Application/Services/Provisioning/ProvisioningScripts/DefaultSchema.sql`
- [ ] Add new tables with proper foreign keys and indexes
- [ ] Update `ProvisioningService` to insert initial company admin user
- [ ] Delete `MasterSchema.sql` if not needed

---

### 1.2 Module Configuration JSON Format

**Current:** String-based storage in `Company.ModulesEnabled` and `Company.SubmodulesEnabled`

**Proposed JSON Structure:**

```json
{
  "modules": [
	{
	  "moduleId": "hr",
	  "moduleName": "Human Resources",
	  "group": "HR",
	  "enabled": true,
	  "submodules": [
		{ "submoduleId": "Employees", "submoduleName": "Employee Management", "enabled": true },
		{ "submoduleId": "Departments", "submoduleName": "Departments", "enabled": true },
		{ "submoduleId": "Recruiting", "submoduleName": "Recruiting", "enabled": false },
		{ "submoduleId": "Performance", "submoduleName": "Performance", "enabled": true },
		{ "submoduleId": "Training", "submoduleName": "Training", "enabled": false },
		{ "submoduleId": "Timekeeping", "submoduleName": "Timekeeping", "enabled": true },
		{ "submoduleId": "HRAdministration", "submoduleName": "HR Administration", "enabled": true },
		{ "submoduleId": "Reports", "submoduleName": "Reports", "enabled": true }
	  ]
	},
	{
	  "moduleId": "crm",
	  "moduleName": "CRM",
	  "group": "Sales",
	  "enabled": true,
	  "submodules": [
		{ "submoduleId": "Leads", "submoduleName": "Leads", "enabled": true },
		{ "submoduleId": "Opportunities", "submoduleName": "Opportunities", "enabled": true },
		{ "submoduleId": "Customers", "submoduleName": "Customers", "enabled": true },
		{ "submoduleId": "Activities", "submoduleName": "Activities", "enabled": false }
	  ]
	}
  ],
  "version": "1.0",
  "lastUpdated": "2026-08-18T00:00:00Z"
}
```

**Benefits:**
- Granular submodule control
- Versioning for schema changes
- Easy to extend with metadata (pricing, feature flags, etc.)
- Query and filter enabled modules easily

**Implementation:**
- [ ] Create `ModuleConfiguration` C# DTO/model classes
- [ ] Update `Company.ModulesEnabled` → use `ModuleRegistry.ModuleConfiguration` JSON column
- [ ] Update provisioning flow to serialize/deserialize JSON
- [ ] Migrate existing string-based data (or keep backward compatibility)

---

## 📋 **Phase 2: Distributed Module Schema System**

### 2.1 Module-Specific Provisioning Scripts

**Goal:** Each microservice/module hosts its own schema script that runs during tenant provisioning.

**Architecture:**

```
services/
├── HR/
│   └── HR.Infrastructure/
│       └── ProvisioningScripts/
│           └── HRModuleSchema.sql
├── LearningManagement/
│   └── LMS.Infrastructure/
│       └── ProvisioningScripts/
│           └── LMSModuleSchema.sql
├── CRM/
│   └── CRM.Infrastructure/
│       └── ProvisioningScripts/
│           └── CRMModuleSchema.sql
├── Inventory/
│   └── Inventory.Infrastructure/
│       └── ProvisioningScripts/
│           └── InventoryModuleSchema.sql
```

**Script Structure Template:**

```sql
-- ============================================================
-- HR Module Provisioning Script
-- Checks if module is enabled and tables don't exist
-- ============================================================

-- Only run if HR module is enabled for this tenant
IF EXISTS (
	SELECT 1 FROM ModuleRegistry MR
	CROSS APPLY OPENJSON(MR.ModuleConfiguration, '$.modules')
	WITH (
		moduleId NVARCHAR(50) '$.moduleId',
		enabled BIT '$.enabled'
	)
	WHERE moduleId = 'hr' AND enabled = 1
)
BEGIN
	-- Create HR tables if they don't exist
	IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HR_Employees')
	BEGIN
		CREATE TABLE HR_Employees (
			Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
			CompanyId UNIQUEIDENTIFIER NOT NULL,
			FirstName NVARCHAR(100) NOT NULL,
			LastName NVARCHAR(100) NOT NULL,
			Email NVARCHAR(255),
			-- ... all other columns
			CONSTRAINT FK_HR_Employees_CompanyInfo FOREIGN KEY (CompanyId) REFERENCES CompanyInfo(Id)
		);

		CREATE INDEX IX_HR_Employees_CompanyId ON HR_Employees(CompanyId);
	END

	-- Create other HR tables (Departments, PerformanceReviews, etc.)
	-- ...
END
GO
```

**Actions:**
- [ ] Create `ProvisioningScripts` folder in each microservice's Infrastructure project
- [ ] Write module-specific SQL scripts for:
  - [ ] HR (Employees, Departments, PerformanceReviews, Training, Timekeeping)
  - [ ] LMS (Courses, Quizzes, Certificates, LearnerProgress, Badges, LearningPaths)
  - [ ] CRM (Leads, Opportunities, Activities, EmailTemplates)
  - [ ] Inventory (Products, Warehouses, Stock, PurchaseOrders, Suppliers)
  - [ ] Finance (Accounts, Transactions, Invoices, Payments)
  - [ ] Sales (Quotes, Orders, Customers)
  - [ ] Services (ServiceCatalog, Appointments, Providers)
- [ ] Update `ProvisioningService` to discover and execute module scripts
- [ ] Add script versioning/migration support (for updates after initial provision)

---

### 2.2 Module Script Orchestration

**Update `ProvisioningService` Flow:**

```csharp
public async Task<ProvisioningResult> ProvisionTenantAsync(ProvisioningRequest request)
{
	// 1. Create tenant database
	await _db.CreateTenantDatabaseAsync(tenantDbName);

	// 2. Apply base schema (DefaultSchema.sql)
	await _db.ApplyTenantSchemaAsync(tenantDbName, baseSchema);

	// 3. Insert company record
	await _db.SaveCompanyInfoAsync(company);

	// 4. Insert module configuration JSON
	await _db.SaveModuleConfigurationAsync(companyId, request.ModuleConfiguration);

	// 5. Discover and apply module scripts
	foreach (var module in request.EnabledModules)
	{
		var scriptPath = _moduleScriptResolver.GetModuleScriptPath(module.ModuleId);
		if (File.Exists(scriptPath))
		{
			var script = await File.ReadAllTextAsync(scriptPath);
			await _db.ApplyTenantSchemaAsync(tenantDbName, script);
		}
	}

	// 6. Create admin user
	await _db.CreateAdminUserAsync(tenantDbName, request.AdminEmail, generatedPassword);

	// 7. Seed initial data (optional)
	await SeedInitialDataAsync(tenantDbName, request);

	return result;
}
```

**Actions:**
- [ ] Create `IModuleScriptResolver` interface
- [ ] Implement `ModuleScriptResolver` to locate scripts by moduleId
- [ ] Update `ProvisioningService` to orchestrate module provisioning
- [ ] Add logging and error handling per module
- [ ] Support rollback on failure

---

## 📋 **Phase 3: Multi-Tenancy Wiring (CompanyId Everywhere)**

### 3.1 Add CompanyId to All Entities

**Modules to Update:**

- [ ] **HR Module**
  - `services/HR/HR.Domain/Entities/Employee.cs` → Add `CompanyId`
  - `services/HR/HR.Domain/Entities/Department.cs` → Add `CompanyId`
  - `services/HR/HR.Domain/Entities/PerformanceReview.cs` → Add `CompanyId`
  - Update all other HR entities
- [ ] **LMS Module** (already documented in `TODO_MULTI_TENANCY.md`)
  - `services/LearningManagement/LMS.Domain/Entities/Course.cs` → Add `CompanyId`
  - `services/LearningManagement/LMS.Domain/Entities/Quiz.cs` → Add `CompanyId`
  - Update all LMS entities
- [ ] **CRM Module**
  - Add `CompanyId` to all CRM entities (Leads, Opportunities, Customers, etc.)
- [ ] **Inventory Module**
  - Add `CompanyId` to all Inventory entities (Products, Warehouses, etc.)
- [ ] **Finance Module**
  - Add `CompanyId` to all Finance entities
- [ ] **Sales Module**
  - Add `CompanyId` to all Sales entities
- [ ] **Services Module**
  - Add `CompanyId` to all Services entities

**Approach:**
- Option A: Add to existing `BaseEntity` (if all entities inherit from it)
- Option B: Create `IMultiTenant` interface and apply selectively
- Option C: Separate base class `TenantEntity : BaseEntity`

---

### 3.2 Update Repositories & DbContexts

**For Each Module:**

- [ ] Update DbContext `OnModelCreating` to add `CompanyId` column and indexes
- [ ] Create EF Core migration to add `CompanyId` to existing tables
- [ ] Update all repository queries to filter by `CompanyId`
  ```csharp
  public async Task<IEnumerable<Employee>> GetAllAsync(Guid companyId)
  {
	  return await _context.Employees
		  .Where(e => e.CompanyId == companyId && !e.IsDeleted)
		  .ToListAsync();
  }
  ```
- [ ] Add `CompanyId` validation in `AddAsync` methods

---

### 3.3 Application Layer - Company Context Service

**Create `ICurrentCompanyService`:**

```csharp
public interface ICurrentCompanyService
{
	Guid GetCurrentCompanyId();
	Task<Company> GetCurrentCompanyAsync();
}

public class CurrentCompanyService : ICurrentCompanyService
{
	private readonly IHttpContextAccessor _httpContext;

	public Guid GetCurrentCompanyId()
	{
		// Extract CompanyId from authenticated user claims
		var companyIdClaim = _httpContext.HttpContext?.User
			.FindFirst("CompanyId")?.Value;

		return Guid.Parse(companyIdClaim ?? throw new UnauthorizedAccessException());
	}
}
```

**Update Handlers:**
- [ ] Inject `ICurrentCompanyService` into all command/query handlers
- [ ] Pass `CompanyId` to repository calls
- [ ] Add company-based authorization checks

---

### 3.4 API Controllers - Extract CompanyId from Auth

**Update Each Module's Controllers:**

```csharp
[ApiController]
[Route("api/hr/employees")]
[Authorize] // ← Re-enable once auth is ready
public class EmployeesController : ControllerBase
{
	private readonly ICurrentCompanyService _companyService;

	[HttpGet]
	public async Task<IActionResult> GetAll()
	{
		var companyId = _companyService.GetCurrentCompanyId();
		var employees = await _employeeRepo.GetAllAsync(companyId);
		return Ok(employees);
	}
}
```

**Actions:**
- [ ] Update all API controllers to use `ICurrentCompanyService`
- [ ] Re-enable `[Authorize]` attributes
- [ ] Add company claim to JWT tokens during authentication
- [ ] Test cross-company data isolation

---

## 📋 **Phase 4: Enhanced Module/Submodule Selection UI**

### 4.1 Update Admin Provisioning UI

**Current:** Admin can select entire modules only.  
**Goal:** Allow granular submodule selection with checkboxes.

**UI Design (Razor View):**

```razor
@foreach (var group in Model.GroupedModules)
{
	<div class="module-group">
		<h4>@group.GroupName</h4>

		@foreach (var module in group.Modules)
		{
			<div class="module-card">
				<input type="checkbox" 
					   id="module-@module.ModuleId" 
					   name="Modules" 
					   value="@module.ModuleId"
					   onchange="toggleSubmodules(this)" />
				<label for="module-@module.ModuleId">
					<strong>@module.DisplayName</strong>
				</label>

				<div class="submodules" id="submodules-@module.ModuleId" style="display:none;">
					@foreach (var sub in module.Submodules)
					{
						<div class="submodule-item">
							<input type="checkbox" 
								   id="sub-@sub.SubmoduleId" 
								   name="Submodules" 
								   value="@module.ModuleId:@sub.SubmoduleId" />
							<label for="sub-@sub.SubmoduleId">@sub.DisplayName</label>
						</div>
					}
				</div>
			</div>
		}
	</div>
}

<script>
function toggleSubmodules(checkbox) {
	var moduleId = checkbox.value;
	var submodulesDiv = document.getElementById('submodules-' + moduleId);
	submodulesDiv.style.display = checkbox.checked ? 'block' : 'none';
}
</script>
```

**Actions:**
- [ ] Update `frontend/BusinessAsUsual.Admin/Views/Company/ProvisionCompany.cshtml`
- [ ] Add JavaScript for "select all" / "deselect all" per module
- [ ] Update `ProvisionCompanyViewModel` to capture selected submodules
- [ ] Build JSON from selections in controller POST action

---

### 4.2 Mirror in Web Signup Flow

**Current:** No signup flow in `BusinessAsUsual.Web`.  
**Goal:** Create customer-facing signup with same module/submodule selection.

**New Pages/Components:**

- [ ] `/signup` page (Blazor)
- [ ] Module selection wizard (multi-step form)
- [ ] Billing plan selection
- [ ] Payment integration (future)
- [ ] Confirmation and account creation

**Blazor Component Structure:**

```
frontend/BusinessAsUsual.Web/
└── Pages/
	└── Signup/
		├── Index.razor           # Step 1: Company info
		├── ModuleSelection.razor # Step 2: Module/submodule picker
		├── BillingPlan.razor     # Step 3: Plan selection
		└── Confirmation.razor    # Step 4: Review and submit
```

**Actions:**
- [ ] Create Blazor signup wizard
- [ ] Reuse `ModuleCatalog` from Core
- [ ] Call provisioning API from Web app
- [ ] Add email verification flow
- [ ] Create tenant login after provisioning

---

## 📋 **Phase 5: Testing & Validation**

### 5.1 Multi-Tenancy Isolation Tests

- [ ] Create integration tests for cross-company data access
- [ ] Verify users cannot access other companies' data
- [ ] Test module/submodule filtering
- [ ] Validate CompanyId in all queries

### 5.2 Provisioning Flow Tests

- [ ] Test end-to-end provisioning from Admin UI
- [ ] Test module script execution
- [ ] Test rollback on failure
- [ ] Test module configuration JSON serialization/deserialization

### 5.3 Performance Tests

- [ ] Benchmark provisioning time with multiple modules
- [ ] Test concurrent provisioning requests
- [ ] Optimize script execution if needed

---

## 🗂️ **File Structure Changes**

```
backend/
├── BusinessAsUsual.Application/
│   └── Services/Provisioning/ProvisioningScripts/
│       ├── DefaultSchema.sql  ← ENHANCED
│       └── MasterSchema.sql   ← DELETE if not needed
│
├── BusinessAsUsual.Infrastructure/
│   └── Provisioning/
│       ├── ProvisioningService.cs  ← UPDATED
│       ├── ModuleScriptResolver.cs ← NEW
│       └── CurrentCompanyService.cs ← NEW
│
├── BusinessAsUsual.Domain/
│   └── Entities/
│       ├── Company.cs  ← ADD ModuleConfiguration JSON
│       └── ModuleConfiguration.cs  ← NEW DTO
│
services/
├── HR/HR.Infrastructure/
│   └── ProvisioningScripts/
│       └── HRModuleSchema.sql ← NEW
│
├── LearningManagement/LMS.Infrastructure/
│   └── ProvisioningScripts/
│       └── LMSModuleSchema.sql ← NEW
│
├── CRM/CRM.Infrastructure/
│   └── ProvisioningScripts/
│       └── CRMModuleSchema.sql ← NEW
│
(... repeat for each module)

frontend/
├── BusinessAsUsual.Admin/
│   └── Views/Company/
│       └── ProvisionCompany.cshtml  ← ENHANCED UI
│
└── BusinessAsUsual.Web/
	└── Pages/Signup/  ← NEW
		├── Index.razor
		├── ModuleSelection.razor
		├── BillingPlan.razor
		└── Confirmation.razor
```

---

## 📊 **Success Criteria**

- [x] DefaultSchema.sql has all core tables with proper CompanyId columns
- [ ] Each module has its own provisioning script
- [ ] All entities across all modules have CompanyId
- [ ] All repositories filter by CompanyId
- [ ] Provisioning service orchestrates base + module scripts
- [ ] Module configuration stored as JSON
- [ ] Admin UI allows granular submodule selection
- [ ] Web signup flow mirrors Admin provisioning
- [ ] Multi-tenancy isolation validated with tests
- [ ] Demo data only visible to demo company (CompanyId filtering)

---

## 🚀 **Implementation Order**

### **Sprint 1: Core Foundation**
1. Enhance `DefaultSchema.sql` with all base tables
2. Add `ModuleRegistry` table and JSON configuration
3. Create `ModuleConfiguration` C# models
4. Update `ProvisioningService` to use JSON

### **Sprint 2: Distributed Provisioning**
5. Create module schema scripts (HR, LMS, CRM, etc.)
6. Implement `ModuleScriptResolver`
7. Update `ProvisioningService` orchestration
8. Test module script execution

### **Sprint 3: Multi-Tenancy Wiring**
9. Add `CompanyId` to all domain entities
10. Create EF migrations for CompanyId columns
11. Update all repositories to filter by CompanyId
12. Implement `ICurrentCompanyService`
13. Update API controllers to use company context

### **Sprint 4: UI Enhancements**
14. Update Admin provisioning UI for submodule selection
15. Build Web signup flow (Blazor wizard)
16. Test provisioning from both UIs

### **Sprint 5: Testing & Polish**
17. Write multi-tenancy isolation tests
18. Performance testing and optimization
19. Documentation and deployment

---

## 📝 **Next Steps**

Which phase/sprint would you like to start with? I recommend:

**Option A:** Start with Sprint 1 (Core Foundation) - enhance DefaultSchema.sql and JSON configuration  
**Option B:** Start with Sprint 2 (Distributed Provisioning) - create module scripts first  
**Option C:** Start with Sprint 3 (Multi-Tenancy) - wire up CompanyId everywhere first

Let me know and we'll dive in! 🎉
