# Payroll Configuration Feature

## Overview
Added comprehensive payroll configuration management allowing administrators to set employee wages, tax rates, and deduction rates.

## Changes Made

### Backend (Finance.Application & Finance.API)

#### New DTOs (`Finance.Application/DTOs/PayrollConfigDtos.cs`)
- `EmployeeWageDto` - Individual employee wage configuration
- `PayrollRatesDto` - Global payroll rates (default wage, tax, deductions)
- `UpdateEmployeeWageRequest` - Request to update employee wage
- `UpdatePayrollRatesRequest` - Request to update global rates

#### Service Changes
- `IPayrollService` - Added configuration methods:
  - `GetEmployeeWagesAsync()` - List all employee-specific wages
  - `GetPayrollRatesAsync()` - Get global rates
  - `UpdateEmployeeWageAsync()` - Update individual employee wage
  - `UpdatePayrollRatesAsync()` - Update global rates

- `PayrollService` - Implemented all configuration methods with logging

- `PayrollDataStore` - Changed rate properties to mutable (set) to allow updates:
  - `DefaultHourlyRate`
  - `TaxRate`
  - `DeductionRate`

#### Controller Endpoints (`PayrollController`)
- `GET /api/finance/payroll/config/wages` - Get all employee wages
- `GET /api/finance/payroll/config/rates` - Get global rates
- `PUT /api/finance/payroll/config/wages/{employeeId}` - Update employee wage
- `PUT /api/finance/payroll/config/rates` - Update global rates

### Frontend (Finance.Web)

#### PayRuns View Updates (`PayRuns.razor`)
- Added columns for Taxes, Deductions, and Net Pay
- Color-coded the new columns (red for taxes, orange for deductions, green for net pay)

#### New Configuration Page (`PayrollConfig.razor`)
Route: `/finance/payroll/config`

Features:
- **Global Rates Panel**
  - Edit default hourly rate
  - Edit tax rate (displayed as percentage)
  - Edit deduction rate (displayed as percentage)
  - Save all rates with one button

- **Employee Wages Panel**
  - List all employees with custom wages
  - Add new employee wage
  - Edit existing employee wage
  - Dialog-based editing UI

#### Navigation Updates (`Payroll.razor`)
- Added "Configuration" card linking to `/finance/payroll/config`

## Usage

### Setting Global Rates
1. Navigate to Finance → Payroll → Configuration
2. In the "Global Rates" panel, update:
   - Default Hourly Rate (used for employees without specific rates)
   - Tax Rate (percentage applied to gross pay)
   - Other Deductions (percentage for benefits, etc.)
3. Click "Save Global Rates"

### Managing Employee Wages
1. Navigate to Finance → Payroll → Configuration
2. In the "Employee Wages" panel:
   - Click "Add Employee" to set a custom rate for a new employee
   - Click the edit icon next to an employee to update their rate
3. Enter Employee ID and Hourly Rate
4. Click "Save"

### Viewing Enhanced Pay Runs
1. Navigate to Finance → Payroll → Pay Runs
2. The pay runs table now shows:
   - Gross Pay (total before deductions)
   - Taxes (red, calculated using tax rate)
   - Deductions (orange, calculated using deduction rate)
   - Net Pay (green, gross - taxes - deductions)

## Data Flow
1. Employee clocks in/out → HR records punches
2. Employee clocks out for the day → HR submits timesheet to Finance via event bus
3. Finance receives timesheet and holds as "pending"
4. Administrator clicks "Run Payroll"
5. Finance calculates pay using:
   - Employee-specific wage (if configured) OR default hourly rate
   - Tax rate × gross pay = taxes
   - Deduction rate × gross pay = deductions
   - Net pay = gross - taxes - deductions
6. Pay run stored with all calculated amounts

## Future Enhancements
- Persist configuration to database (currently in-memory)
- Employee selection dropdown (integrate with HR employee data)
- More sophisticated tax brackets
- Multiple deduction types (benefits, retirement, etc.)
- Pay period configuration
- Export pay runs to CSV/Excel
- Integration with external payroll providers
