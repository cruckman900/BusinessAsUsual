# Module Catalog - Unified Reference

## Purpose
This document provides a single source of truth for all modules in the BusinessAsUsual platform, merging:
- **ModuleCatalog.cs** (conceptual groups/submodules)
- **ModuleDiscoveryService.GetFallbackModules()** (runtime navigation with icons/routes)

**Authority**: GetFallbackModules supersedes ModuleCatalog when conflicts exist.

---

## Active Modules (Implemented & In Shell)

These modules are built, deployed, and accessible through the shell:

| Module | Key | Group | Route | Icon | Port(API) | Port(HTTPS) | Status |
|--------|-----|-------|-------|------|-----------|-------------|---------|
| **HR** | `hr` | HR & People | `/hr` | `Icons.Material.Filled.People` | 7200 | 7201 | ✅ Active |
| **Finance** | `finance` | Financial | `/finance` | `Icons.Material.Filled.AttachMoney` | 7100 | 7101 | ✅ Active |
| **CRM** | `crm` | Sales & CRM | `/crm` | `Icons.Material.Filled.ContactPhone` | 7300 | 7301 | ✅ Active |
| **Inventory** | `inventory` | Operations | `/inventory` | `Icons.Material.Filled.Inventory2` | 7250 | 7251 | ✅ Active |
| **Sales** | `sales` | Sales & CRM | `/sales` | `Icons.Material.Filled.ShoppingCart` | 7050 | 7051 | ✅ Active |
| **Services** | `services` | Operations | `/services` | `Icons.Material.Filled.MiscellaneousServices` | 7286 | 7285 | ✅ Active |

---

## Platform Modules (System-Level, To Be Built)

These are cross-cutting system modules defined in ModuleCatalog but not yet implemented:

| Module | Key | Submodules | Priority | Notes |
|--------|-----|------------|----------|-------|
| **User Management** | `usermanagement` | Users, Roles, Permissions | High | Core auth/authz |
| **Audit Logs** | `auditlogs` | System Events, Security Events | High | Compliance |
| **Notifications** | `notifications` | Email, SMS, Push | Medium | Cross-module |
| **Reporting** | `reporting` | Dashboards, Exports, KPIs | Medium | Analytics |
| **Integrations** | `integrations` | API Keys, Webhooks, Connectors | Low | Extensibility |
| **Settings** | `settings` | Company Profile, Preferences | Medium | Config |
| **Localization** | `localization` | Languages, Regions | Low | i18n |

---

## Module Navigation Hierarchy (From GetFallbackModules)

### HR Module
```
Home (/hr)
├── Employee Management
│   ├── All Employees (/hr/employees)
│   └── Add Employee (/hr/employees/new)
├── Departments
│   ├── All Departments (/hr/departments)
│   └── Add Department (/hr/departments/new)
├── Recruiting
│   ├── Applicants (/hr/applicants)
│   └── Interviews (/hr/interviews)
├── Performance
│   ├── Reviews (/hr/reviews)
│   └── Goals (/hr/goals)
├── Training
│   ├── Courses (/hr/courses)
│   └── Certifications (/hr/certifications)
├── Timekeeping
│   ├── Timesheets (/hr/timesheets)
│   └── Approvals (/hr/approvals)
├── HR Administration
│   ├── Onboarding (/hr/onboarding)
│   └── Benefits (/hr/benefits)
└── Reports (/hr/reports)
```

### Finance Module
```
Home (/finance)
├── Accounts Receivable
│   ├── Invoices (/finance/invoices)
│   └── Collections (/finance/receivables/collections)
├── Accounts Payable
│   ├── Bills (/finance/payables/bills)
│   └── Vendor Payments (/finance/payables/vendor-payments)
├── General Ledger
│   ├── Chart of Accounts (/finance/gl/chart-of-accounts)
│   ├── Journal Entries (/finance/gl/journal-entries)
│   └── Trial Balance (/finance/gl/trial-balance)
├── Banking (/finance/banking)
├── Payments (/finance/payments)
├── Payroll (/finance/payroll)
└── Reports (/finance/reports)
```

### CRM Module
```
Home (/crm)
├── Leads
│   ├── All Leads (/crm/leads)
│   └── Add Lead (/crm/leads/new)
├── Opportunities
│   ├── All Opportunities (/crm/opportunities)
│   ├── Pipeline Board (/crm/pipeline)
│   └── Add Opportunity (/crm/opportunities/new)
├── Customers
│   ├── All Customers (/crm/customers)
│   └── Add Customer (/crm/customers/new)
├── Activities (/crm/activities)
├── Email Templates (/crm/email-templates)
├── Reports (/crm/reports)
└── Settings (/crm/settings)
```

### Inventory Module
```
Home (/inventory)
├── Products
│   ├── All Products (/inventory/products)
│   └── Add Product (/inventory/products/new)
├── Warehouses
│   ├── All Warehouses (/inventory/warehouses)
│   └── Add Warehouse (/inventory/warehouses/new)
├── Stock Management (/inventory/stock)
├── Purchase Orders (/inventory/purchase-orders)
├── Suppliers (/inventory/suppliers)
└── Reports (/inventory/reports)
```

### Sales Module
```
Home (/sales)
├── Quotes
│   ├── All Quotes (/sales/quotes)
│   └── Create Quote (/sales/quotes/new)
├── Orders
│   ├── All Orders (/sales/orders)
│   └── Create Order (/sales/orders/new)
├── Customers
│   ├── All Customers (/sales/customers)
│   └── Add Customer (/sales/customers/new)
└── Reports (/sales/reports)
```

### Services Module
```
Home (/services)
├── Service Catalog (/services/list)
├── Providers (/services/providers) [Coming Soon]
├── Appointments (/services/appointments) [Coming Soon]
└── Reports (/services/reports) [Coming Soon]
```

---

## Planned Modules (ModuleCatalog Only)

Modules defined in ModuleCatalog but not yet in GetFallbackModules:

### Financial (Extended)
- Billing, Banking, Budgeting, Taxation

### Sales & CRM (Extended)
- Customers, Quotes, Orders, Subscriptions, POS, Products, Menu, Customer Portal

### Operations (Extended)
- Warehousing, Purchasing, Procurement, Suppliers, Equipment, Maintenance, Vehicles
- Fleet Management, Logistics, Routing, Scheduling, Projects, Tasks, Jobs, Workflows
- Replenishment, Forecasting, Quality Control, Compliance, Asset Management

### Documents & Communication
- Documents, Messaging, Knowledge Base, File Storage

### Industry-Specific
- Healthcare (Patients, Clinical Notes)
- Hospitality (Reservations, Events)
- Mining (Safety)
- Professional Services (Contracts, Field Service)

---

## Maintenance Protocol

**CRITICAL**: When adding or modifying a module, update BOTH:

1. ✅ **ModuleCatalog.cs** (`BusinessAsUsual.Core/Modules/ModuleCatalog.cs`)
   - Add/update module definition with Group, Key, Name, Submodules

2. ✅ **GetFallbackModules()** (`frontend/BusinessAsUsual.Web/Services/ModuleDiscoveryService.cs`)
   - Add/update ModuleDto with full navigation hierarchy, icons, routes

**Format for ModuleCatalog.cs:**
```csharp
new("GroupName", "ModuleKey", "Display Name", new []
{
	new SubmoduleDefinition("SubKey", "Sub Display Name"),
	// ...
})
```

**Format for GetFallbackModules():**
```csharp
new ModuleDto
{
	ModuleId = "modulekey",
	Key = "modulekey",
	DisplayName = "Display Name",
	Description = "Short description",
	UiEntryPoint = "/modulekey",
	Icon = Icons.Material.Filled.IconName,
	IsActive = true,
	NavigationItems = new List<NavigationItemDto>
	{
		new() { Label = "Home", Route = "/modulekey", Icon = Icons.Material.Filled.Home },
		// ... groups with Children
	}
}
```

---

## Icon Color Guidelines Reference

When adding module dashboard submodule cards:

| Color | MudBlazor Value | Use Cases |
|-------|----------------|-----------|
| **Primary** (Blue) | `Color.Primary` | Core features, main workflows |
| **Success** (Green) | `Color.Success` | Positive actions, relationships |
| **Info** (Cyan) | `Color.Info` | Infrastructure, locations |
| **Warning** (Amber) | `Color.Warning` | Monitoring, alerts |
| **Tertiary** (Purple) | `Color.Tertiary` | Secondary workflows |
| **Secondary** (Dark) | `Color.Secondary` | Analytics, reporting |
| **Default** (Gray) | `Color.Default` | Coming soon features |

---

## Port Allocation

See `docs/PORT_REGISTRY.md` for authoritative port assignments.

**Current Ranges:**
- Platform Services: 5000-5099
- Business Modules HTTP: 7000-7399
- Business Modules HTTPS: 7000-7399
- **Platform Module (Reserved)**: 7400-7499

---

*Last Updated: [Auto-generated during plan execution]*
