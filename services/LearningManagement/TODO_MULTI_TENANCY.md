# LMS Multi-Tenancy Implementation TODO

## Current State
- LMS entities (Course, Quiz, Assignment, etc.) do NOT have `CompanyId` fields
- All LMS data is currently global/shared across all users
- Demo seed data is not yet enabled in production (waiting for multi-tenancy)

## Required Changes

### 1. Add CompanyId to Core Entities
Add `CompanyId` field to:
- [ ] `Course`
- [ ] `Quiz`
- [ ] `Assignment`
- [ ] `LearnerProgress`
- [ ] `Certificate`
- [ ] `LearningPath`
- [ ] `Badge`
- [ ] `Notification`

### 2. Update BaseEntity or Create Multi-Tenant Base
Option A: Add to BaseEntity
```csharp
public abstract class BaseEntity
{
	public Guid Id { get; set; }
	public string CompanyId { get; set; } = string.Empty; // Multi-tenant isolation
	// ... existing properties
}
```

Option B: Create separate IMultiTenant interface
```csharp
public interface IMultiTenant
{
	string CompanyId { get; set; }
}
```

### 3. Update Database Schema
- [ ] Create migration to add `CompanyId` column to all LMS tables
- [ ] Add foreign key constraints or indexes as needed

### 4. Update Repositories
- [ ] Add `CompanyId` filtering to all repository queries
- [ ] Update `GetAllAsync()` methods to filter by company
- [ ] Add company validation to `AddAsync()` methods

### 5. Update Application Layer
- [ ] Add `ICurrentCompanyService` or similar to get current user's company
- [ ] Update command/query handlers to pass `CompanyId`
- [ ] Add company authorization checks

### 6. Update API Controllers
- [ ] Extract `CompanyId` from authenticated user claims
- [ ] Pass `CompanyId` to handlers/services
- [ ] Ensure users can only access their company's data

### 7. Seeding Strategy
- [ ] Seed demo data with a specific "Demo Company" ID
- [ ] Only show demo data to unauthenticated users or demo company users
- [ ] Allow production seeding with company isolation

### 8. Testing
- [ ] Add multi-tenancy isolation tests
- [ ] Verify users cannot access other companies' LMS data
- [ ] Test demo data visibility rules

## Related Files
- `services/LearningManagement/LMS.Domain/Entities/BaseEntity.cs`
- `services/LearningManagement/LMS.Domain/Entities/Course.cs` (and all other entities)
- `services/LearningManagement/LMS.Infrastructure/Persistence/LMSDbContext.cs`
- `services/LearningManagement/LMS.Infrastructure/Repositories/*`
- `frontend/BusinessAsUsual.Web/Program.cs` (seeding logic)

## Priority
**HIGH** - Required before enabling production LMS seeding and proper multi-company usage

## Estimated Effort
2-3 days for full implementation and testing
