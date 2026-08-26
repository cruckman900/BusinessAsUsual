# Chapter 3: Platform & Core Features

[CHAPTER START - Page 18]

## 3.1 Platform Module Overview

The **Platform Module** provides the foundational infrastructure and shared services that all other modules depend on. It includes system administration, user management, data import/export capabilities, audit logging, and system configuration.

### Key Capabilities

- ✅ **User & Role Management**: Create users, assign roles, manage permissions
- ✅ **Data Import System**: Advanced CSV/Excel import with column mapping
- ✅ **Import History**: Track all data imports with detailed logs
- ✅ **Mapping Templates**: Save and reuse import configurations
- ✅ **Audit Trails**: System-wide change tracking
- ✅ **System Configuration**: Application settings and preferences
- ✅ **Module Registry**: Dynamic module discovery and management
- 🔧 **Multi-Tenancy**: Tenant isolation and management (in progress)
- 📋 **Notification System**: Email, SMS, in-app notifications (planned)
- 📋 **Workflow Engine**: Configurable approval workflows (planned)

---

## 3.2 User Management

### 3.2.1 User Accounts

**Purpose**: Manage system access for all personnel

**Key Features**:
- Create, edit, deactivate user accounts
- Password management and reset
- Multi-factor authentication (MFA) support
- Login history and session management
- Account locking after failed attempts

**User Information**:
- Email (username)
- First and last name
- Phone number
- Preferred language
- Time zone
- Profile photo

**User States**:
- **Active**: Can login and access system
- **Inactive**: Temporarily disabled
- **Locked**: Auto-locked after failed logins
- **Pending**: Invited but not yet activated

---

### 3.2.2 Roles & Permissions

**Role Hierarchy**:

```
Super Admin (System-wide access)
  ├── Tenant Admin (Org-wide access)
  │    ├── Module Admin (Module management)
  │    ├── Manager (Department oversight)
  │    └── Coordinator (Data entry + limited oversight)
  └── Employee (Self-service + assigned tasks)
	   └── Guest (Read-only, limited access)
```

**Permission Model**:

Permissions are granular and can be assigned by:
- **Module**: Access to specific modules (HR, Sales, Finance)
- **Entity**: Access to specific data types (Employees, Customers, Invoices)
- **Action**: What operations are allowed (Create, Read, Update, Delete, Approve)

**Example Permission Set**:
```
Module: HR
Entity: Employees
Actions: Read, Update (but not Create or Delete)
Scope: Department (can only see own department)
```

**Common Role Configurations**:

| Role | Typical Permissions |
|------|---------------------|
| **Super Admin** | All modules, all actions, system configuration |
| **HR Manager** | HR module: all actions; View employees across departments |
| **HR Coordinator** | HR module: Create/Update employees, process payroll |
| **Sales Manager** | Sales + CRM: all actions; View team performance |
| **Sales Rep** | Sales + CRM: Create quotes/orders, manage own opportunities |
| **Accountant** | Finance: all actions; Read-only inventory, sales |
| **Employee** | Self-service: View own data, submit time off, update profile |

---

### 3.2.3 User Onboarding Workflow

**Step 1: Invitation**
- Admin sends email invitation
- User receives secure link with token
- Link expires after 48 hours

**Step 2: Account Setup**
- User clicks link, arrives at setup page
- Sets password (must meet complexity requirements)
- Optionally enables MFA
- Reviews and accepts terms of service

**Step 3: Profile Completion**
- Upload profile photo
- Set time zone and preferences
- Complete required profile fields
- Take initial tour of system

**Step 4: Access Granted**
- Account activated
- Initial notification sent
- User can begin working

---

## 3.3 Data Import System

One of the most powerful features of the Platform module is the **Advanced Data Import System**, allowing bulk data migration from legacy systems or regular data updates from external sources.

### 3.3.1 Import Wizard Overview

The import wizard guides users through a multi-step process:

**Step 1: Select Target**
- Choose which table/entity to import (Employees, Customers, Products, etc.)
- System displays required fields and data types
- Option to load saved mapping template

**Step 2: Upload File**
- Supports CSV, Excel (.xlsx), Tab-delimited
- Drag-and-drop or file browse
- File validation and preview
- Shows first 10 rows for review

**Step 3: Column Mapping**
- Auto-detection of column matches
- Manual mapping for unmatched columns
- Shows required vs. optional fields
- Identifies auto-generated fields (ID, Created Date)
- Visual indicators for missing required fields

**Step 4: Data Preview**
- Shows how data will import
- Highlights potential issues
- Validation errors displayed
- Option to fix issues or proceed with warnings

**Step 5: Import Execution**
- Batch processing (configurable batch size)
- Real-time progress indicator
- Detailed logging of each row
- Error handling options (skip, abort, or continue)

**Step 6: Results & Review**
- Summary: Total records, successful, failed, skipped
- Download error report
- View import in History for reference

---

### 3.3.2 Column Mapping Intelligence

The system intelligently maps source columns to target fields:

**Automatic Mapping**:
- Exact match: `FirstName` → `FirstName`
- Case-insensitive: `firstname` → `FirstName`
- Common variations: `FName` → `FirstName`, `Email Address` → `Email`

**Smart Field Detection**:
- **Date fields**: Recognizes various date formats (MM/DD/YYYY, YYYY-MM-DD, etc.)
- **Phone numbers**: Accepts (555) 123-4567, 555-123-4567, +1-555-123-4567
- **Currency**: Handles $1,234.56, 1234.56, 1,234.56
- **Boolean**: Yes/No, True/False, 1/0, Y/N

**Auto-Generated Fields**:
The system identifies and excludes auto-generated fields from required validation:
- Primary keys (ID, EmployeeId)
- System timestamps (CreatedDate, ModifiedDate)
- Auto-calculated fields (Age from DOB)
- Tenant identifiers

**Unmapped Columns**:
Source columns that don't match any target field are shown separately:
- Can be manually mapped
- Can be ignored (data not imported)
- Saved in mapping template for future use

---

### 3.3.3 Data Transformation

Apply transformations during import:

**String Transformations**:
- Trim whitespace
- Convert to uppercase/lowercase
- Remove special characters
- Replace patterns (regex supported)

**Date Transformations**:
- Parse custom date formats
- Adjust time zones
- Add/subtract days (e.g., effective dates)

**Number Transformations**:
- Unit conversion (e.g., pounds to kilograms)
- Currency conversion
- Rounding/precision

**Lookup Transformations**:
- Map codes to values (e.g., "M" → "Male", "F" → "Female")
- Reference data lookups (e.g., department name → department ID)

**Example Transformation**:
```
Source: phone = "555.123.4567"
Transform: Remove dots, format as (XXX) XXX-XXXX
Result: (555) 123-4567
```

---

### 3.3.4 Mapping Templates

Save mapping configurations for reuse:

**Template Contents**:
- Source-to-target column mappings
- Transformation rules
- Default values for missing data
- Validation rules
- Import settings (batch size, error handling)

**Template Management**:
- Create template during import
- Save with descriptive name
- Load template in future imports
- Share templates with team
- Version history

**Use Cases**:
- **Weekly payroll imports**: Save template for recurring files
- **Monthly sales data**: Consistent format from external system
- **Onboarding batches**: HR uses same template for new hire CSV
- **Partner data feeds**: Standardize vendor data formats

---

### 3.3.5 Import History & Audit

Every import is logged with complete details:

**Import Record Contains**:
- Timestamp
- User who performed import
- Target table
- Source file name
- Mapping template used (if any)
- Total records processed
- Success/failure/skip counts
- Detailed logs

**Import Status**:
- **Completed**: All rows processed successfully
- **Completed with Errors**: Some rows failed
- **Failed**: Import did not complete
- **In Progress**: Currently running (for large files)

**Detailed Logs**:
- Row-by-row results
- Error messages with line numbers
- Validation failures
- Transformation applied

**Import History Page**:
- Searchable list of all imports
- Filter by user, table, date range, status
- View details for any import
- Re-run failed import (with corrections)
- Download original file
- Download error report

**Example Import Summary**:
```
Import ID: IMP-20260826-0001
Table: Employees
File: NewHires_Aug2026.csv
User: hr.manager@example.com
Date: 2026-08-26 10:35 AM
Status: Completed with Errors

Results:
- Total Rows: 25
- Successful: 23
- Failed: 2
- Skipped: 0

Errors:
Row 5: Email validation failed - "invalid-email" is not valid
Row 12: Required field missing - HireDate is required
```

---

## 3.4 Schema Introspection Service

Behind the scenes, the Platform module uses **Schema Introspection** to dynamically understand database structures.

**Capabilities**:
- Query entity metadata from Entity Framework
- Identify table structures, columns, data types
- Detect primary keys, foreign keys, indexes
- Recognize required vs. optional fields
- Identify auto-generated columns
- Understand validation rules

**Benefits**:
- No hard-coded import configurations
- Automatically supports new tables/columns
- Validates data before import
- Provides user-friendly field descriptions

**How It Works**:
```csharp
// System queries EF Core metadata
var entityType = dbContext.Model.FindEntityType(typeof(Employee));
var properties = entityType.GetProperties();

foreach (var property in properties)
{
	var column = new ColumnSchema
	{
		Name = property.Name,
		DataType = property.ClrType,
		IsRequired = !property.IsNullable,
		IsAutoGenerated = property.ValueGenerated != ValueGenerated.Never,
		MaxLength = property.GetMaxLength()
	};
}
```

---

## 3.5 Audit System

### 3.5.1 Audit Trail Overview

Every create, update, and delete operation across all modules is logged:

**Captured Information**:
- **Who**: User who performed the action
- **What**: Entity type and ID
- **When**: Timestamp (UTC)
- **Where**: Module and table
- **How**: Old value → New value

**Audit Entry Example**:
```json
{
  "auditId": "AUD-20260826-0001",
  "timestamp": "2026-08-26T14:30:00Z",
  "userId": "user-guid",
  "userName": "jane.smith@example.com",
  "module": "HR",
  "entity": "Employee",
  "entityId": "emp-guid",
  "action": "Update",
  "changes": [
	{
	  "field": "Salary",
	  "oldValue": "75000",
	  "newValue": "80000"
	},
	{
	  "field": "JobTitle",
	  "oldValue": "Developer",
	  "newValue": "Senior Developer"
	}
  ]
}
```

---

### 3.5.2 Audit Search & Reporting

**Search Capabilities**:
- Filter by date range
- Filter by user
- Filter by entity type
- Filter by action (Create/Update/Delete)
- Search by entity ID
- Full-text search on changes

**Audit Reports**:
- User activity report (what did user X do?)
- Entity history (all changes to record Y)
- Compliance reports (who changed sensitive data?)
- Change summary (bulk update tracking)

**Compliance Features**:
- Tamper-proof logs (write-once, immutable)
- Retention policies (keep for 7 years)
- Export to secure archive
- Access logging (who viewed audit logs?)

---

## 3.6 System Configuration

### 3.6.1 Application Settings

**General Settings**:
- Company name and logo
- Default time zone
- Default currency
- Date/time format
- Language preferences

**Email Settings**:
- SMTP configuration
- Email templates
- Sender address
- Reply-to address

**Security Settings**:
- Password complexity requirements
- Session timeout
- MFA enforcement
- IP whitelisting
- Rate limiting

**Integration Settings**:
- API keys for third-party services
- Webhook URLs
- OAuth configuration
- External service credentials

---

### 3.6.2 Feature Flags

Enable/disable features without code changes:

**Available Flags**:
- `EnableAdvancedImport`: Show advanced import options
- `EnableMFA`: Require multi-factor authentication
- `EnableAuditLog`: Capture audit trail
- `EnableAPIAccess`: Allow API key generation
- `EnableWebhooks`: Allow webhook registration

**Use Cases**:
- Gradual rollout of new features
- A/B testing
- Emergency feature disable
- Per-tenant customization

---

## 3.7 Module Registry

### 3.7.1 Dynamic Module Discovery

The Platform maintains a registry of all installed modules:

**Module Metadata**:
- Name and version
- Description
- Author/vendor
- Dependencies
- Enabled/disabled status
- Health check endpoint

**Module Capabilities**:
- Advertise API endpoints
- Register navigation menu items
- Declare data entities for import
- Publish integration events
- Request permissions

---

### 3.7.2 Module Management

**Install Module**:
1. Upload module package
2. Validate dependencies
3. Run database migrations
4. Register with platform
5. Enable module

**Uninstall Module**:
1. Disable module
2. Remove menu items
3. Deregister endpoints
4. Optionally purge data
5. Remove package

**Update Module**:
1. Upload new version
2. Run migrations
3. Update registry
4. Restart services

---

## 3.8 Platform API Reference

### 3.8.1 User Management API

**Create User**:
```http
POST /api/v1/users
Content-Type: application/json

{
  "email": "new.user@example.com",
  "firstName": "New",
  "lastName": "User",
  "roles": ["Employee"]
}
```

**Update User**:
```http
PUT /api/v1/users/{id}
Content-Type: application/json

{
  "firstName": "Updated",
  "lastName": "Name",
  "phoneNumber": "(555) 123-4567"
}
```

**Assign Roles**:
```http
POST /api/v1/users/{id}/roles
Content-Type: application/json

{
  "roles": ["HR Manager", "Employee"]
}
```

---

### 3.8.2 Import API

**Create Import Job**:
```http
POST /api/v1/import
Content-Type: multipart/form-data

{
  "targetTable": "Employees",
  "file": [binary content],
  "mappingTemplateId": "template-guid" (optional)
}
```

**Get Import Status**:
```http
GET /api/v1/import/{importId}

Response:
{
  "importId": "imp-guid",
  "status": "Completed",
  "totalRecords": 100,
  "successCount": 98,
  "errorCount": 2,
  "progress": 100
}
```

**Download Error Report**:
```http
GET /api/v1/import/{importId}/errors

Response: CSV file with error details
```

---

## 3.9 Common Tasks & Workflows

### 3.9.1 Onboard New Employee with Import

**Scenario**: HR has a CSV file with 20 new hires

**Steps**:
1. Navigate to Platform → Import
2. Select target: "Employees"
3. Upload `NewHires.csv`
4. Review auto-mapped columns
5. Manually map any unmapped columns (e.g., "Start Date" → "HireDate")
6. Preview data
7. Execute import
8. Review results
9. Save mapping template as "New Hire Template"
10. Send welcome emails to successful imports

**Time**: 5-10 minutes (vs. 2+ hours manual entry)

---

### 3.9.2 Bulk Update from External System

**Scenario**: Payroll system exports salary updates monthly

**Steps**:
1. Navigate to Platform → Import
2. Select target: "Employees"
3. Load saved template: "Monthly Salary Update"
4. Upload current month's file
5. System auto-maps using saved template
6. Preview shows only changed salaries
7. Execute import
8. System updates salary fields
9. Audit log captures all changes
10. Finance receives notification of updates

**Time**: 2-3 minutes

---

### 3.9.3 Audit Investigation

**Scenario**: CFO asks "Who changed the budget for Marketing last month?"

**Steps**:
1. Navigate to Platform → Audit Log
2. Filter by:
   - Date range: Last 30 days
   - Entity: "Budget"
   - Field contains: "Marketing"
3. Review results
4. Find entry showing user `john.doe@example.com` changed budget from $50K to $60K on Aug 15
5. Export audit report for records

**Time**: 1-2 minutes

---

## 3.10 Best Practices

### Import Best Practices

✅ **Always Preview**: Review data preview before importing  
✅ **Start Small**: Test with 10-20 records first  
✅ **Save Templates**: Create reusable templates for recurring imports  
✅ **Clean Data First**: Fix data issues in source file before importing  
✅ **Use Mapping**: Don't rely only on auto-detection  
✅ **Check History**: Review past imports for patterns  
✅ **Download Errors**: Always download and fix error reports

❌ **Don't Import Duplicates**: Check for existing records first  
❌ **Don't Skip Preview**: Can catch issues early  
❌ **Don't Ignore Warnings**: May indicate data quality issues  
❌ **Don't Mix Data Types**: Keep dates as dates, numbers as numbers

---

### Security Best Practices

✅ **Enable MFA**: Especially for admin accounts  
✅ **Review Permissions**: Regularly audit role assignments  
✅ **Rotate API Keys**: Change keys quarterly  
✅ **Monitor Audit Logs**: Review for suspicious activity  
✅ **Strong Passwords**: Enforce complexity requirements  
✅ **Session Timeout**: 30 minutes of inactivity

❌ **Don't Share Accounts**: Each user should have their own  
❌ **Don't Use Weak Passwords**: Enforce strong policy  
❌ **Don't Leave Sessions**: Always log out on shared computers  
❌ **Don't Over-Privilege**: Grant minimum needed permissions

---

## 3.11 Troubleshooting

### Import Issues

**Problem**: "File upload failed"  
**Solution**: Check file size limit (default 50MB), ensure correct format

**Problem**: "Required field missing"  
**Solution**: Map the required field or provide default value in template

**Problem**: "Data type mismatch"  
**Solution**: Verify source data matches target data type (e.g., dates are valid)

**Problem**: "Duplicate key error"  
**Solution**: Check for duplicate records in source file, use update mode instead of insert

---

### User Access Issues

**Problem**: "User cannot login"  
**Solution**: Check account status (Active), verify password, check for account lock

**Problem**: "User cannot see module"  
**Solution**: Verify user has role with access to that module

**Problem**: "User gets permission denied"  
**Solution**: Check role permissions for specific entity/action

---

## 3.12 Summary

The Platform module is the **foundation of Business As Usual**, providing:

✅ Centralized user and access management  
✅ Powerful data import capabilities with intelligence  
✅ Complete audit trail for compliance  
✅ System configuration and customization  
✅ Module extensibility framework

**Key Features**:
- Advanced import wizard with auto-mapping
- Mapping templates for recurring imports
- Comprehensive audit logging
- Role-based security
- Dynamic module registry

**Next Chapter**: **Chapter 4 - Human Resources Module**  
Explore the full HR capabilities including employee management, payroll, and benefits.

---

[CHAPTER END - Estimated 15 pages]

[Page Break]
