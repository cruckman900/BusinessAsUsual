# Mobile UI Contract System

## Overview

The Business As Usual platform implements a **Mobile UI Contract** system that allows mobile applications (Android & iOS) to dynamically discover and construct their UI based on specifications provided by each microservice.

## Architecture: "Service = API + UI + Mobile Contract"

Each microservice provides three components:

1. **REST API** - Backend endpoints for data operations
2. **Web UI** - Blazor components injected into the web app
3. **Mobile Contract** - JSON specifications that tell mobile apps how to render UI

## How It Works

```
┌─────────────────────────────────────────────────────┐
│                  Mobile Application                 │
│                  (Android/iOS)                      │
└─────────────────┬───────────────────────────────────┘
                  │
                  │ 1. Query MRS for modules
                  ▼
┌─────────────────────────────────────────────────────┐
│          Module Registry Service (MRS)              │
│          Returns: Mobile-enabled modules             │
│          with MobileUISpecUrl                       │
└─────────────────┬───────────────────────────────────┘
                  │
                  │ 2. Fetch UI spec from each module
                  ▼
┌─────────────────────────────────────────────────────┐
│               HR Microservice                        │
│         GET /api/hr/mobile/ui-spec                  │
│         Returns: Complete UI specification          │
└─────────────────────────────────────────────────────┘
                  │
                  │ 3. Mobile app renders UI based on spec
                  ▼
          ┌──────────────────┐
          │  Native Android  │
          │    or iOS UI     │
          └──────────────────┘
```

## Mobile UI Specification Structure

### 1. Module Registration with Mobile Support

When a service registers with MRS, it includes mobile contract information:

```json
{
  "moduleId": "hr",
  "displayName": "Human Resources",
  "apiBaseUrl": "http://localhost:5001",
  "uiEntryPoint": "http://localhost:5002/hr",
  "supportsMobile": true,
  "mobileUISpecUrl": "http://localhost:5001/api/hr/mobile/ui-spec",
  "mobileContractVersion": "1.0.0"
}
```

### 2. Mobile Apps Query MRS

Mobile apps can discover mobile-enabled modules:

```
GET /api/modules/mobile
```

Response:
```json
[
  {
    "moduleId": "hr",
    "displayName": "Human Resources",
    "supportsMobile": true,
    "mobileUISpecUrl": "http://localhost:5001/api/hr/mobile/ui-spec",
    "mobileContractVersion": "1.0.0"
  }
]
```

### 3. Mobile Apps Fetch UI Specifications

For each module, mobile apps fetch the UI specification:

```
GET /api/hr/mobile/ui-spec
```

Response:
```json
{
  "moduleId": "hr",
  "moduleName": "Human Resources",
  "version": "1.0.0",
  "navigation": {
    "items": [
      {
        "id": "employees",
        "label": "Employees",
        "icon": "people",
        "screen": "employee-list",
        "route": "/hr/employees"
      }
    ]
  },
  "screens": {
    "employee-list": { ... },
    "employee-detail": { ... },
    "employee-form": { ... }
  }
}
```

## Screen Specifications

### Employee List Screen

```
GET /api/hr/mobile/ui-spec/employee-list
```

Response:
```json
{
  "title": "Employees",
  "searchPlaceholder": "Search employees...",
  "enableSearch": true,
  "enableFilter": true,
  "columns": [
    {
      "name": "photoUrl",
      "label": "Photo",
      "type": "image",
      "width": 60
    },
    {
      "name": "fullName",
      "label": "Name",
      "type": "text",
      "sortable": true,
      "width": 200
    }
  ],
  "actions": [
    {
      "id": "add",
      "label": "Add Employee",
      "icon": "add",
      "action": "navigate",
      "navigateTo": "/hr/employees/new"
    }
  ],
  "filters": [
    {
      "id": "department",
      "label": "Department",
      "type": "select",
      "values": [
        { "id": "eng", "label": "Engineering", "value": "engineering" }
      ]
    }
  ]
}
```

### Employee Detail Screen

```
GET /api/hr/mobile/ui-spec/employee-detail
```

Response:
```json
{
  "title": "Employee Details",
  "sections": [
    {
      "id": "personal",
      "title": "Personal Information",
      "fields": [
        {
          "name": "fullName",
          "label": "Full Name",
          "type": "text"
        },
        {
          "name": "email",
          "label": "Email",
          "type": "email",
          "icon": "email"
        }
      ]
    }
  ],
  "actions": [
    {
      "id": "edit",
      "label": "Edit",
      "icon": "edit",
      "action": "navigate",
      "navigateTo": "/hr/employees/{id}/edit"
    }
  ]
}
```

### Employee Form Screen

```
GET /api/hr/mobile/ui-spec/employee-form
```

Response:
```json
{
  "title": "Employee Information",
  "sections": [
    {
      "id": "personal",
      "title": "Personal Information",
      "fields": [
        {
          "name": "firstName",
          "label": "First Name",
          "type": "text",
          "required": true,
          "maxLength": 50,
          "validationMessage": "First name is required"
        },
        {
          "name": "email",
          "label": "Email",
          "type": "email",
          "required": true,
          "pattern": "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$",
          "validationMessage": "Please enter a valid email"
        },
        {
          "name": "departmentId",
          "label": "Department",
          "type": "select",
          "required": true,
          "options": [
            { "value": "1", "label": "Engineering" },
            { "value": "2", "label": "Sales" }
          ]
        }
      ]
    }
  ],
  "actions": [
    {
      "id": "save",
      "label": "Save",
      "icon": "save",
      "action": "api-call",
      "apiEndpoint": "/api/employees"
    }
  ],
  "validation": {
    "errorMessages": {
      "required": "This field is required",
      "email": "Please enter a valid email address"
    }
  }
}
```

## Field Types

The mobile UI contract supports various field types:

| Type | Description | Example Use |
|------|-------------|-------------|
| `text` | Plain text input | Name, address |
| `email` | Email input with validation | Email address |
| `phone` | Phone number input | Phone number |
| `number` | Numeric input | Age, quantity |
| `date` | Date picker | Hire date, birthday |
| `select` | Dropdown selection | Department, status |
| `multiselect` | Multiple selections | Skills, tags |
| `image` | Image display/upload | Profile photo |
| `badge` | Status badge | Active, Pending |

## Action Types

Actions define what happens when a user interacts with UI elements:

| Action Type | Description | Required Fields |
|-------------|-------------|-----------------|
| `navigate` | Navigate to another screen | `navigateTo` |
| `api-call` | Call an API endpoint | `apiEndpoint` |
| `custom` | Custom action handled by app | `customHandler` |

## Mobile App Implementation

### Android (Kotlin) Example

```kotlin
// 1. Discover modules
val modules = api.getModulesWithMobile()

// 2. Fetch UI spec for HR module
val hrSpec = api.getMobileUISpec("hr")

// 3. Render navigation
renderNavigation(hrSpec.navigation)

// 4. Fetch screen spec
val employeeListSpec = api.getEmployeeListSpec()

// 5. Render list screen
fun renderEmployeeList(spec: EmployeeListSpec, data: List<Employee>) {
    // Create RecyclerView with columns from spec
    spec.columns.forEach { column ->
        when (column.type) {
            "image" -> addImageColumn(column)
            "text" -> addTextColumn(column)
            "badge" -> addBadgeColumn(column)
        }
    }

    // Add search if enabled
    if (spec.enableSearch) {
        addSearchBar(spec.searchPlaceholder)
    }

    // Add filters
    spec.filters.forEach { filter ->
        addFilterControl(filter)
    }

    // Add actions
    spec.actions.forEach { action ->
        addActionButton(action)
    }
}
```

### iOS (Swift) Example

```swift
// 1. Discover modules
let modules = try await api.getModulesWithMobile()

// 2. Fetch UI spec
let hrSpec = try await api.getMobileUISpec(moduleId: "hr")

// 3. Render list screen
func renderEmployeeList(spec: EmployeeListSpec, data: [Employee]) {
    // Create UITableView with spec
    let tableView = UITableView()

    // Configure columns
    spec.columns.forEach { column in
        switch column.type {
        case "image":
            configureImageColumn(column)
        case "text":
            configureTextColumn(column)
        case "badge":
            configureBadgeColumn(column)
        default:
            break
        }
    }

    // Add search if enabled
    if spec.enableSearch {
        addSearchController(placeholder: spec.searchPlaceholder)
    }

    // Add actions
    spec.actions.forEach { action in
        addBarButtonItem(for: action)
    }
}
```

## Benefits

✅ **No Hardcoded UI** - Mobile apps don't need to know screen structure  
✅ **Consistent UI/UX** - Backend defines UI rules  
✅ **Easy Updates** - Change UI without app updates  
✅ **Platform Agnostic** - Same spec works for Android & iOS  
✅ **Validation Sync** - Validation rules defined once  
✅ **Feature Parity** - Web and mobile always in sync  

## Contract Versioning

The mobile contract includes versioning support:

```json
{
  "mobileContractVersion": "1.0.0"
}
```

Mobile apps can:
- Check contract version before rendering
- Handle version mismatches gracefully
- Update UI based on contract changes

## Available Endpoints

### Module Registry Service

- `GET /api/modules/mobile` - Get mobile-enabled modules

### HR Service

- `GET /api/hr/mobile/ui-spec` - Complete UI specification
- `GET /api/hr/mobile/navigation` - Navigation structure
- `GET /api/hr/mobile/ui-spec/employee-list` - Employee list spec
- `GET /api/hr/mobile/ui-spec/employee-detail` - Employee detail spec
- `GET /api/hr/mobile/ui-spec/employee-form` - Employee form spec

## Project Structure

```
HR.Contracts/
├── UIModels/
│   ├── EmployeeViewModel.cs
│   └── DepartmentViewModel.cs
├── Specifications/
│   ├── EmployeeListSpec.cs
│   ├── EmployeeDetailSpec.cs
│   └── EmployeeFormSpec.cs
└── Navigation/
    └── HRNavigationMap.cs

HR.API/
└── Controllers/
    └── MobileUIController.cs
```

## Testing Mobile Contracts

Use the provided `.http` file to test mobile endpoints:

```http
### Get mobile-enabled modules
GET http://localhost:5100/api/modules/mobile

### Get HR mobile UI spec
GET http://localhost:5001/api/hr/mobile/ui-spec

### Get employee list spec
GET http://localhost:5001/api/hr/mobile/ui-spec/employee-list

### Get navigation
GET http://localhost:5001/api/hr/mobile/navigation
```

## Next Steps

1. **Implement in Mobile Apps**
   - Create spec parsers
   - Build dynamic renderers
   - Handle all field types

2. **Add More Modules**
   - Accounting mobile contracts
   - Inventory mobile contracts
   - CRM mobile contracts

3. **Enhance Specifications**
   - Add localization support
   - Add theming support
   - Add conditional visibility rules

4. **Create SDKs**
   - Android SDK for consuming specs
   - iOS SDK for consuming specs
   - React Native SDK

## Documentation

- [Mobile UI Contract Implementation](./archive/MOBILE_UI_CONTRACT_IMPLEMENTATION.md) _(archived)_
- [Module Registry Guide](./MODULE_REGISTRY_AND_UI_INJECTION.md)
- [Microservice Architecture](./MICROSERVICEARCHITECTUREOVERVIEW.md)

---

**The "Service = API + UI + Mobile Contract" vision is now complete!** 🚀
