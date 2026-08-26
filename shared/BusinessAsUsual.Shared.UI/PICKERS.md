# Shared UI Component Library - Picker Components

## Overview
All reusable picker components are now centralized in `shared/BusinessAsUsual.Shared.UI/Components/`. These components provide autocomplete-based selection for common entities across the BusinessAsUsual application.

## Available Pickers

### CustomerPicker
A customer selection component with three search modes:
- **Individual**: Search for individual customers
- **Company**: Search for companies only
- **All**: Search across all customers and companies

**Usage:**
```razor
<CustomerPicker @bind-SelectedCustomer="@_customer"
				Label="Select Customer"
				Required="true"
				RequiredError="Please select a customer" />
```

**Parameters:**
- `SelectedCustomer` (CustomerDto?) - The selected customer
- `SelectedCustomerChanged` (EventCallback<CustomerDto?>) - Fires when selection changes
- `Label` (string) - Field label (default: "Select Customer")
- `Variant` (Variant) - MudBlazor variant (default: Outlined)
- `Margin` (Margin) - Field margin (default: Dense)
- `Dense` (bool) - Use dense layout (default: true)
- `Required` (bool) - Mark as required (default: false)
- `RequiredError` (string) - Required validation message
- `Disabled` (bool) - Disable the picker

**Dependencies:** CRM.Application (ICustomerService, CustomerDto)

---

### ProductPicker
A product selection component with search across name, SKU, description, and category.

**Usage:**
```razor
<ProductPicker @bind-SelectedProduct="@_product"
			   Label="Select Product"
			   Required="true"
			   RequiredError="Please select a product" />
```

**Parameters:**
- `SelectedProduct` (ProductDto?) - The selected product
- `SelectedProductChanged` (EventCallback<ProductDto?>) - Fires when selection changes
- `Label` (string) - Field label (default: "Select Product")
- `Variant` (Variant) - MudBlazor variant (default: Outlined)
- `Margin` (Margin) - Field margin (default: Dense)
- `Dense` (bool) - Use dense layout (default: true)
- `Required` (bool) - Mark as required (default: false)
- `RequiredError` (string) - Required validation message
- `Disabled` (bool) - Disable the picker

**Dependencies:** Inventory.Application (IInventoryService, ProductDto)

---

### EmployeePicker
An employee selection component with optional active-only filtering.

**Usage:**
```razor
<EmployeePicker @bind-SelectedEmployee="@_employee"
				Label="Select Employee"
				ActiveOnly="true"
				Required="true" />
```

**Parameters:**
- `SelectedEmployee` (EmployeeDto?) - The selected employee
- `SelectedEmployeeChanged` (EventCallback<EmployeeDto?>) - Fires when selection changes
- `Label` (string) - Field label (default: "Select Employee")
- `Variant` (Variant) - MudBlazor variant (default: Outlined)
- `Margin` (Margin) - Field margin (default: Dense)
- `Dense` (bool) - Use dense layout (default: true)
- `Required` (bool) - Mark as required (default: false)
- `RequiredError` (string) - Required validation message
- `Disabled` (bool) - Disable the picker
- `ActiveOnly` (bool) - Show only active employees (default: true)

**Dependencies:** HR.Application (IEmployeeService, EmployeeDto)

---

### DepartmentPicker
A department selection component with search across name, description, and location.

**Usage:**
```razor
<DepartmentPicker @bind-SelectedDepartment="@_department"
				  Label="Select Department"
				  Required="true" />
```

**Parameters:**
- `SelectedDepartment` (DepartmentDto?) - The selected department
- `SelectedDepartmentChanged` (EventCallback<DepartmentDto?>) - Fires when selection changes
- `Label` (string) - Field label (default: "Select Department")
- `Variant` (Variant) - MudBlazor variant (default: Outlined)
- `Margin` (Margin) - Field margin (default: Dense)
- `Dense` (bool) - Use dense layout (default: true)
- `Required` (bool) - Mark as required (default: false)
- `RequiredError` (string) - Required validation message
- `Disabled` (bool) - Disable the picker

**Dependencies:** HR.Application (IDepartmentService, DepartmentDto)

---

## Setup for Consuming Projects

1. **Add Project Reference** to your `.csproj`:
```xml
<ProjectReference Include="..\..\..\shared\BusinessAsUsual.Shared.UI\BusinessAsUsual.Shared.UI.csproj" />
```

2. **Add Namespace Import** to your `_Imports.razor`:
```razor
@using BusinessAsUsual.Shared.UI.Components
@using MudBlazor
```

3. **Use the Components** in your Razor pages/components.

---

## Features

All pickers share these common features:
- **Autocomplete**: Fast, intelligent search with debouncing (300ms)
- **Smart Filtering**: Searches across multiple relevant fields
- **Top Results**: Shows top 20 results by default when opened
- **Prioritized Sorting**: Results starting with search text appear first
- **Loading States**: Shows loading indicator and helpful messages
- **Clearable**: Built-in clear button to reset selection
- **Accessible**: Proper ARIA labels and keyboard navigation
- **Customizable**: Configurable labels, variants, validation, and more

---

## Architecture Notes

The `BusinessAsUsual.Shared.UI` library references:
- `CRM.Application` (for customer data)
- `HR.Application` (for employee and department data)
- `Inventory.Application` (for product data)

This means any project that references the shared UI library will transitively get access to these application layers. This is intentional to provide a seamless experience for form-building across modules.

---

## Migration Status

✅ **CustomerPicker** - Migrated from Sales.Web, now in shared library  
✅ **ProductPicker** - Migrated from Sales.Web, now in shared library  
✅ **EmployeePicker** - Created new in shared library  
✅ **DepartmentPicker** - Created new in shared library  
✅ **Sales.Web** - Updated to use shared pickers  

Other modules can now reference these pickers as needed.
