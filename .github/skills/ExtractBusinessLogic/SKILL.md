# SKILL: Extract Business Logic from Blazor Components

## Objective
Move business logic from Blazor Razor components into testable application services to improve code coverage, maintainability, and reusability.

## The Problem
Blazor components with embedded business logic are difficult to unit test:
- **MudBlazor dependencies** require complex test harness setup (providers, popover services, etc.)
- **Component rendering** involves service providers, JS interop, and lifecycle management
- **Business logic** (calculations, exports, validation) is buried in component `@code` blocks
- **Coverage suffers** because Web/UI projects have 0% coverage due to testing complexity

## The Solution: Service Extraction Pattern

Extract business logic from Razor components into dedicated application layer services that can be unit tested independently.

---

## Pattern Example: CSV Export

### Before (Hard to Test)
```razor
@page "/hr/employees"
@using System.Text
@inject IJSRuntime JS

@code {
	private IEnumerable<EmployeeDto>? employees;

	private async Task ExportToCsv()
	{
		var employeesToExport = employees ?? Enumerable.Empty<EmployeeDto>();

		var csv = new StringBuilder();
		csv.AppendLine("Full Name,Email,Department,Hire Date");

		foreach (var employee in employeesToExport)
		{
			var dept = employee.Department ?? "";
			csv.AppendLine($"\"{employee.FirstName} {employee.LastName}\",\"{employee.Email}\",\"{dept}\",\"{employee.HireDate:yyyy-MM-dd}\"");
		}

		var bytes = Encoding.UTF8.GetBytes(csv.ToString());
		var base64 = Convert.ToBase64String(bytes);
		var fileName = $"employees-export-{DateTime.Now:yyyy-MM-dd}.csv";

		await JS.InvokeVoidAsync("downloadFile", fileName, base64, "text/csv");
		Toast.Success($"Exported {employeesToExport.Count()} employee(s) to CSV");
	}
}
```

**Problems:**
- 18 lines of business logic embedded in component
- CSV generation logic not unit testable
- Requires bUnit + MudBlazor test infrastructure to test
- Logic cannot be reused in API endpoints or batch jobs

### After (Testable)

**1. Create Interface** (`HR.Application/Services/IExportService.cs`):
```csharp
using HR.Application.DTOs;

namespace HR.Application.Services;

public interface IExportService
{
	/// <summary>
	/// Generate CSV content from employee data
	/// </summary>
	(string Base64Content, string FileName) GenerateEmployeeCsv(IEnumerable<EmployeeDto> employees);
}
```

**2. Implement Service** (`HR.Application/Services/ExportService.cs`):
```csharp
using System.Text;
using HR.Application.DTOs;

namespace HR.Application.Services;

public class ExportService : IExportService
{
	public (string Base64Content, string FileName) GenerateEmployeeCsv(IEnumerable<EmployeeDto> employees)
	{
		var employeeList = employees?.ToList() ?? new List<EmployeeDto>();

		var csv = new StringBuilder();
		csv.AppendLine("Full Name,Email,Department,Hire Date");

		foreach (var employee in employeeList)
		{
			var dept = employee.Department ?? "";
			csv.AppendLine($"\"{employee.FirstName} {employee.LastName}\",\"{employee.Email}\",\"{dept}\",\"{employee.HireDate:yyyy-MM-dd}\"");
		}

		var bytes = Encoding.UTF8.GetBytes(csv.ToString());
		var base64 = Convert.ToBase64String(bytes);
		var fileName = $"employees-export-{DateTime.Now:yyyy-MM-dd}.csv";

		return (base64, fileName);
	}
}
```

**3. Write Unit Tests** (`HR.Application.Tests/Services/ExportServiceTests.cs`):
```csharp
using FluentAssertions;
using HR.Application.Services;

public class ExportServiceTests
{
	[Fact]
	public void GenerateEmployeeCsv_Should_Generate_Valid_CSV_With_Headers()
	{
		// Arrange
		var service = new ExportService();
		var employees = new List<EmployeeDto>
		{
			new() { FirstName = "John", LastName = "Doe", Email = "john@example.com", 
					Department = "Engineering", HireDate = new DateTime(2020, 1, 15) }
		};

		// Act
		var (base64Content, fileName) = service.GenerateEmployeeCsv(employees);

		// Assert
		base64Content.Should().NotBeNullOrEmpty();
		fileName.Should().StartWith("employees-export-");

		var csvBytes = Convert.FromBase64String(base64Content);
		var csvContent = Encoding.UTF8.GetString(csvBytes);
		csvContent.Should().Contain("Full Name,Email,Department,Hire Date");
		csvContent.Should().Contain("\"John Doe\",\"john@example.com\",\"Engineering\",\"2020-01-15\"");
	}

	[Fact]
	public void GenerateEmployeeCsv_Should_Handle_Null_Input()
	{
		var service = new ExportService();
		var (base64, fileName) = service.GenerateEmployeeCsv(null!);

		base64.Should().NotBeNullOrEmpty();
		var csv = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
		csv.Should().Contain("Full Name,Email,Department,Hire Date");
	}
}
```

**4. Register in DI** (`HR.Web/Program.cs`):
```csharp
builder.Services.AddScoped<IExportService, ExportService>();
```

**5. Refactor Component** (`HR.Web/Components/Pages/EmployeeList.razor`):
```razor
@page "/hr/employees"
@inject IExportService ExportService
@inject IJSRuntime JS

@code {
	private IEnumerable<EmployeeDto>? employees;

	private async Task ExportToCsv()
	{
		var employeesToExport = employees ?? Enumerable.Empty<EmployeeDto>();
		var (base64Content, fileName) = ExportService.GenerateEmployeeCsv(employeesToExport);

		await JS.InvokeVoidAsync("downloadFile", fileName, base64Content, "text/csv");
		Toast.Success($"Exported {employeesToExport.Count()} employee(s) to CSV");
	}
}
```

**Benefits:**
- ✅ Component reduced from 18 lines to 3 lines
- ✅ Business logic now unit testable (9 tests added)
- ✅ No MudBlazor/bUnit dependencies needed for tests
- ✅ Logic reusable in API endpoints, batch jobs, etc.
- ✅ Coverage increased in Application layer

---

## When to Extract

### Extract Logic When It Involves:
- ✅ **Data transformation** (CSV, PDF, Excel generation)
- ✅ **Business calculations** (totals, percentages, aggregations, reports)
- ✅ **Complex validation** (multi-field rules, domain constraints)
- ✅ **Data formatting** (date/number formatting, string building)
- ✅ **Algorithms** (sorting, filtering, grouping, calculations)
- ✅ **Domain logic** (state transitions, business rules)

### Keep in Components:
- ❌ **UI state management** (selected items, expanded panels, dialog state)
- ❌ **Navigation logic** (routing, redirects)
- ❌ **Dialog/toast notifications** (user feedback)
- ❌ **JS interop calls** (extract data preparation, keep the invocation)
- ❌ **Component lifecycle** (OnInitialized, OnParametersSet)

---

## Implementation Checklist

When extracting logic from a Razor component:

1. ☐ **Identify** the business logic to extract (calculations, transformations, exports)
2. ☐ **Create interface** in `{Module}.Application/Services/I{Service}.cs`
3. ☐ **Implement service** in `{Module}.Application/Services/{Service}.cs`
   - Service should have **no UI dependencies** (no IJSRuntime, NavigationManager, etc.)
   - Return data structures (tuples, DTOs) that the component can use
4. ☐ **Write unit tests** in `{Module}.Application.Tests/Services/{Service}Tests.cs`
   - Test happy path
   - Test null/empty input handling
   - Test edge cases (special characters, large data sets, etc.)
5. ☐ **Register in DI** in `{Module}.Web/Program.cs` or `{Module}.API/Program.cs`
   ```csharp
   builder.Services.AddScoped<I{Service}, {Service}>();
   ```
6. ☐ **Refactor component** to inject and use the service
   ```razor
   @inject I{Service} {Service}
   ```
7. ☐ **Verify** component still works (manual testing)
8. ☐ **Run tests** to confirm all tests pass
9. ☐ **Commit** with clear message explaining the extraction

---

## Real-World Example: HR.Web EmployeeList

### What Was Extracted
- **Before:** CSV export logic embedded in `EmployeeList.razor` (18 lines)
- **After:** Logic moved to `ExportService.GenerateEmployeeCsv()` (service layer)

### Impact
- **Tests added:** 9 unit tests (null handling, empty lists, multiple records, etc.)
- **Component simplified:** Export method reduced from 18 lines to 3 lines
- **Coverage improved:** Application layer now testable without MudBlazor/bUnit complexity
- **Reusability:** Same export logic can now be used by:
  - API endpoint: `GET /api/employees/export/csv`
  - Scheduled job: Daily employee report email
  - Batch export: Multi-department export tool

### Test Coverage
```
ExportServiceTests (9 tests):
✅ GenerateEmployeeCsv_Should_Generate_Valid_CSV_With_Headers
✅ GenerateEmployeeCsv_Should_Handle_Empty_List
✅ GenerateEmployeeCsv_Should_Handle_Null_Input
✅ GenerateEmployeeCsv_Should_Handle_Null_Department
✅ GenerateEmployeeCsv_Should_Handle_Multiple_Employees
✅ GenerateDepartmentCsv_Should_Generate_Valid_CSV_With_Headers
✅ GenerateDepartmentCsv_Should_Handle_Empty_List
✅ GenerateDepartmentCsv_Should_Handle_Null_Description
✅ GenerateDepartmentCsv_Should_Handle_Multiple_Departments
```

---

## Common Extraction Patterns

### 1. Export Services
**Extract:** CSV/PDF/Excel generation logic  
**Service:** `IExportService`  
**Methods:** `GenerateEmployeeCsv()`, `GenerateDepartmentPdf()`, etc.

### 2. Calculation Services
**Extract:** Financial calculations, statistical aggregations  
**Service:** `ICalculationService`  
**Methods:** `CalculateTotalRevenue()`, `ComputeAverageSales()`, etc.

### 3. Validation Services
**Extract:** Complex multi-field validation rules  
**Service:** `IValidationService`  
**Methods:** `ValidateEmployeeData()`, `CheckBusinessRules()`, etc.

### 4. Report Services
**Extract:** Report data aggregation and formatting  
**Service:** `IReportService`  
**Methods:** `GenerateHeadcountReport()`, `BuildSalesAnalytics()`, etc.

### 5. Formatting Services
**Extract:** Complex string/date/number formatting  
**Service:** `IFormattingService`  
**Methods:** `FormatCurrency()`, `BuildDisplayName()`, etc.

---

## Benefits Summary

### For Testing
- ✅ **Fast unit tests** (milliseconds vs. seconds for component tests)
- ✅ **No UI dependencies** (no MudBlazor, bUnit, JS interop mocking)
- ✅ **Clear test scenarios** (pure input → output testing)
- ✅ **Easy mocking** (simple interface mocking in integration tests)

### For Maintainability
- ✅ **Separation of concerns** (UI vs. business logic)
- ✅ **Single responsibility** (components handle UI, services handle logic)
- ✅ **Easier refactoring** (change logic without touching UI)
- ✅ **Clear dependencies** (explicit service injection)

### For Reusability
- ✅ **API endpoints** (same logic in Web and API projects)
- ✅ **Batch jobs** (background processing, scheduled tasks)
- ✅ **Integration points** (webhooks, external system exports)
- ✅ **Multiple consumers** (desktop app, mobile app, CLI tools)

### For Coverage
- ✅ **Improved metrics** (Application layer coverage increases)
- ✅ **Meaningful coverage** (business logic covered, not just UI rendering)
- ✅ **CI/CD confidence** (critical logic has automated tests)

---

## Anti-Patterns to Avoid

### ❌ Don't Extract Too Much
```csharp
// BAD: UI state management in a service
public class EmployeeListService
{
	public bool IsDialogOpen { get; set; }  // ❌ UI state belongs in component
	public string SelectedTab { get; set; }  // ❌ Component state
}
```

### ❌ Don't Create Service Dependencies on UI
```csharp
// BAD: Service depends on UI infrastructure
public class ExportService
{
	private readonly IJSRuntime _js;  // ❌ Service shouldn't know about JS interop

	public async Task ExportToCsv()
	{
		await _js.InvokeVoidAsync("download", ...);  // ❌ Should return data, not invoke UI
	}
}
```

### ❌ Don't Over-Engineer Simple Logic
```csharp
// UNNECESSARY: Extracting trivial formatting
public class SimpleFormattingService
{
	public string Capitalize(string input) => input.ToUpper();  // ❌ Too simple to extract
}
```

### ✅ Do Extract Complex, Testable Logic
```csharp
// GOOD: Complex business calculation
public class PayrollService
{
	public decimal CalculateGrossPay(EmployeeDto employee, int hoursWorked)
	{
		var regularPay = Math.Min(hoursWorked, 40) * employee.HourlyRate;
		var overtimePay = Math.Max(0, hoursWorked - 40) * employee.HourlyRate * 1.5m;
		return regularPay + overtimePay;  // ✅ Testable business logic
	}
}
```

---

## Related Skills
- **Testing Blazor Components** - When you DO need to test component rendering
- **Dependency Injection Best Practices** - Service lifetime management
- **Clean Architecture** - Layer separation principles
- **Test-Driven Development** - Writing tests before implementation

---

## Questions Before Starting
- What business logic is currently in the component?
- Does it involve calculations, transformations, or complex rules?
- Could this logic be reused outside the component?
- Can it be tested without UI dependencies?
- What DTOs/models does it operate on?

---

## Success Criteria
After extraction, you should have:
- ✅ Service interface defined
- ✅ Service implementation with no UI dependencies
- ✅ Comprehensive unit tests (>80% coverage of service logic)
- ✅ Service registered in DI container
- ✅ Component refactored to use service
- ✅ All existing tests still passing
- ✅ Component behavior unchanged from user perspective
