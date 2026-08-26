using HR.Domain.Entities;
using HR.Domain.Repositories;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BusinessAsUsual.Application.Services;

namespace HR.Infrastructure.Repositories;

/// <summary>
/// Department repository implementation
/// </summary>
public class DepartmentRepository : IDepartmentRepository
{
    private readonly HRDbContext _context;
    private readonly ITenantContext _tenantContext;

    public DepartmentRepository(HRDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<IEnumerable<Department>> GetAllAsync()
    {
#pragma warning disable CS0618 // Type or member is obsolete - loading legacy Manager navigation for backward compatibility
        return await _context.Departments
            .Where(d => d.CompanyId == _tenantContext.CompanyId)
            .Include(d => d.Manager)
            .Include(d => d.ParentDepartment)
            .Include(d => d.SubDepartments)
            .Include(d => d.DepartmentManagers)
                .ThenInclude(dm => dm.Manager)
            .Include(d => d.EmployeeDepartments)
                .ThenInclude(ed => ed.Employee)
            .OrderBy(d => d.Name)
            .ToListAsync();
#pragma warning restore CS0618
    }

    public async Task<Department?> GetByIdAsync(Guid id)
    {
#pragma warning disable CS0618 // Type or member is obsolete - loading legacy Manager navigation for backward compatibility
        return await _context.Departments
            .Where(d => d.CompanyId == _tenantContext.CompanyId)
            .Include(d => d.Manager)
            .Include(d => d.ParentDepartment)
            .Include(d => d.SubDepartments)
            .Include(d => d.DepartmentManagers)
                .ThenInclude(dm => dm.Manager)
            .Include(d => d.EmployeeDepartments)
                .ThenInclude(ed => ed.Employee)
            .FirstOrDefaultAsync(d => d.Id == id);
#pragma warning restore CS0618
    }

    public async Task<Department> CreateAsync(Department department)
    {
        department.CompanyId = _tenantContext.CompanyId;
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
        return department;
    }

    public async Task<Department> UpdateAsync(Department department)
    {
        // Validate tenant ownership
        var existing = await _context.Departments
            .Where(d => d.Id == department.Id && d.CompanyId == _tenantContext.CompanyId)
            .FirstOrDefaultAsync();

        if (existing == null)
            throw new UnauthorizedAccessException($"Department {department.Id} not found or access denied.");

        _context.Departments.Update(department);
        await _context.SaveChangesAsync();
        return department;
    }

    public async Task DeleteAsync(Guid id)
    {
        var department = await _context.Departments
            .Where(d => d.Id == id && d.CompanyId == _tenantContext.CompanyId)
            .FirstOrDefaultAsync();

        if (department != null)
        {
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
        }
    }
}
