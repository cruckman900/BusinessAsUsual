# Finance Module Enhancements

## Overview
Three major improvements to the Finance module: fixed API connectivity, built out the Deductions submodule, and enhanced employee wage configuration with lookup functionality.

## Changes Made

### 1. Fixed Finance.Web API Connectivity Issue

**Problem:** `Failed to load payroll data: No connection could be made because the target machine actively refused it. (localhost:5007)`

**Root Cause:** Finance.Web was using a default unnamed `HttpClient` while pages expected a named client "FinanceApi".

**Solution:**
- Updated `services/Finance/Finance.Web/Program.cs`:
  - Changed from default `HttpClient` to named client registration
  - Added `AddHttpClient("FinanceApi", ...)` for Finance API calls
  - Added `AddHttpClient("HrApi", ...)` for HR employee lookups

- Updated `services/Finance/Finance.Web/appsettings.json`:
  - Added `HrService:Url` configuration for HR API endpoint

**Files Changed:**
- `services/Finance/Finance.Web/Program.cs`
- `services/Finance/Finance.Web/appsettings.json`

---

### 2. Built Out Deductions Submodule

**Before:** Placeholder page with "coming soon" message.

**After:** Full-featured deductions management and visualization page.

**Features Added:**

#### Current Rate Configuration Panel
- Displays active tax withholding rate (red badge)
- Displays other deductions rate (orange badge)
- Live example calculation showing:
  - Gross pay for 40 hours
  - Tax deduction amount
  - Other deductions amount
  - Net take-home pay

#### Deduction Categories Panel
- Federal Income Tax (included in tax rate)
- State & Local Tax (included in tax rate)
- Health Insurance (other deductions)
- Retirement Contributions (401k, IRA - other deductions)
- Info alert explaining current flat-rate model

#### Deduction Impact Summary Table
- Shows last 5 pay runs
- Columns: Pay Run Date, Employees, Gross Pay, Total Taxes, Total Deductions, Net Pay
- **Take-Home %** column showing what percentage employees actually receive
- Color-coded amounts matching the PayRuns view

**Navigation:**
- "Configure Rates" button links to `/finance/payroll/config` for easy access

**Files Changed:**
- `services/Finance/Finance.Web/Components/Pages/Deductions.razor`

**Route:** `/finance/payroll/deductions`

---

### 3. Enhanced Employee Wage Configuration with Lookup

**Problem:** Users had to manually type employee IDs, which requires knowing the exact ID format.

**Solution:** Added employee lookup with autocomplete search.

**Features Added:**

#### Toggle Between Lookup and Manual Entry
- **Employee Lookup Mode** (default):
  - Autocomplete search box
  - Search by: Employee ID, First Name, Last Name, or Email
  - Shows formatted results: "FirstName LastName (ID)"
  - Displays top 10 matches

- **Manual Entry Mode**:
  - Text field for direct ID entry
  - Useful when HR API is unavailable or for new employees

#### Employee Data Integration
- Fetches employee list from HR.API on page load: `GET /api/hr/employees`
- Falls back gracefully if HR API is unavailable
- Caches employees in memory for fast autocomplete searching

#### UX Improvements
- Toggle switch: "Lookup Employee" / "Enter Employee ID"
- Search icon on autocomplete field
- When editing existing wage, employee ID is locked/disabled

**Files Changed:**
- `services/Finance/Finance.Web/Components/Pages/PayrollConfig.razor`
  - Added employee lookup autocomplete
  - Added toggle for lookup vs manual entry
  - Added `LoadEmployeesAsync()` method
  - Added `SearchEmployeesAsync()` autocomplete function
  - Added `EmployeeLookupDto` internal class

**API Integration:**
- Uses HR.API endpoint: `GET /api/hr/employees`
- Maps to internal `EmployeeLookupDto` with: Id, FirstName, LastName, Email

---

## Usage

### Using Employee Lookup in Payroll Config
1. Navigate to **Finance → Payroll → Configuration**
2. Click **Add Employee** in the Employee Wages panel
3. The dialog defaults to **Lookup Employee** mode
4. Start typing an employee's name, ID, or email
5. Select from the autocomplete suggestions
6. Enter hourly rate and save

**OR**

3. Toggle to **Enter Employee ID** mode
4. Type the employee ID manually
5. Enter hourly rate and save

### Viewing Deductions
1. Navigate to **Finance → Payroll → Deductions**
2. See current tax and deduction rates
3. Review example calculation for 40-hour week
4. View deduction impact across recent pay runs
5. Click **Configure Rates** to adjust settings

---

## Technical Details

### HttpClient Registration Pattern
```csharp
// Named client for Finance API
builder.Services.AddHttpClient("FinanceApi", client =>
{
	client.BaseAddress = new Uri(financeApiUrl);
});

// Named client for HR API (employee lookup)
builder.Services.AddHttpClient("HrApi", client =>
{
	client.BaseAddress = new Uri(hrApiUrl);
});
```

### Autocomplete Search Implementation
```csharp
private async Task<IEnumerable<EmployeeLookupDto>> SearchEmployeesAsync(
	string searchText, 
	CancellationToken cancellationToken)
{
	if (string.IsNullOrWhiteSpace(searchText))
		return _allEmployees.Take(10);

	return _allEmployees
		.Where(e => 
			e.Id.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
			e.FirstName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
			e.LastName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
			e.Email.Contains(searchText, StringComparison.OrdinalIgnoreCase))
		.Take(10);
}
```

---

## Future Enhancements

### Deductions Module
- [ ] Individual deduction line items (per employee)
- [ ] Tax brackets instead of flat rates
- [ ] Multiple deduction types (health, dental, vision, 401k, etc.)
- [ ] Employee benefit elections
- [ ] Pre-tax vs post-tax deductions
- [ ] Employer contribution matching
- [ ] Export deduction reports

### Employee Lookup
- [ ] Show employee department and title in autocomplete
- [ ] Display current wage (if set) when selecting employee
- [ ] Bulk import wages from CSV
- [ ] Wage history tracking
- [ ] Effective date ranges for wage changes

---

## Testing Recommendations

1. **Test Finance.Web connectivity:**
   - Start Finance.API on port 5007
   - Start Finance.Web
   - Navigate to Pay Runs - should load without connection errors

2. **Test employee lookup:**
   - Start HR.API on port 5041
   - Start Finance.Web
   - Go to Payroll Config → Add Employee
   - Search for an employee by name
   - Verify autocomplete shows results

3. **Test manual entry fallback:**
   - Stop HR.API (or use manual mode toggle)
   - Add employee wage using manual ID entry
   - Verify save works correctly

4. **Test deductions page:**
   - Navigate to Deductions
   - Verify rates display correctly
   - Verify example calculation is accurate
   - Run a pay run and verify it appears in impact summary
