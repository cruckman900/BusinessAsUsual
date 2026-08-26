using HR.Domain.Entities;
using HR.Domain.Repositories;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BusinessAsUsual.Application.Services;

namespace HR.Infrastructure.Repositories;

/// <summary>
/// Employee repository implementation
/// </summary>
public class EmployeeRepository : IEmployeeRepository
{
    private readonly HRDbContext _context;
    private readonly ITenantContext _tenantContext;

    public EmployeeRepository(HRDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _context.Employees
            .Where(e => e.CompanyId == _tenantContext.CompanyId)
            .Include(e => e.Manager)
            .Include(e => e.EmployeeDepartments)
                .ThenInclude(ed => ed.Department)
            .Include(e => e.ManagedDepartments)
                .ThenInclude(dm => dm.Department)
            .Include(e => e.TrainingCompletions)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(Guid id)
    {
        return await _context.Employees
            .Where(e => e.CompanyId == _tenantContext.CompanyId)
            .Include(e => e.Manager)
            .Include(e => e.EmployeeDepartments)
                .ThenInclude(ed => ed.Department)
            .Include(e => e.ManagedDepartments)
                .ThenInclude(dm => dm.Department)
            .Include(e => e.DirectReports)
            .Include(e => e.TrainingCompletions)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Employee> CreateAsync(Employee employee)
    {
        employee.CompanyId = _tenantContext.CompanyId;
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee> UpdateAsync(Employee employee)
    {
        // Validate tenant ownership
        var existing = await _context.Employees
            .Where(e => e.Id == employee.Id && e.CompanyId == _tenantContext.CompanyId)
            .FirstOrDefaultAsync();

        if (existing == null)
            throw new UnauthorizedAccessException($"Employee {employee.Id} not found or access denied.");

        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task DeleteAsync(Guid id)
    {
        var employee = await _context.Employees
            .Where(e => e.Id == id && e.CompanyId == _tenantContext.CompanyId)
            .FirstOrDefaultAsync();

        if (employee != null)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Employee>> SearchAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
#pragma warning disable CS0618 // Type or member is obsolete - using legacy Department field for search compatibility
#pragma warning disable CS8602 // Dereference of a possibly null reference
        return await _context.Employees
            .Where(e => e.CompanyId == _tenantContext.CompanyId)
            .Where(e => e.FirstName.ToLower().Contains(term) ||
                       e.LastName.ToLower().Contains(term) ||
                       e.Email.ToLower().Contains(term) ||
                       e.Department.ToLower().Contains(term))
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync();
#pragma warning restore CS8602
#pragma warning restore CS0618
    }
}
