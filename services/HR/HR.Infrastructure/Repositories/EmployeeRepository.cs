using HR.Domain.Entities;
using HR.Domain.Repositories;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Repositories;

/// <summary>
/// Employee repository implementation
/// </summary>
public class EmployeeRepository : IEmployeeRepository
{
    private readonly HRDbContext _context;

    public EmployeeRepository(HRDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _context.Employees
            .Include(e => e.Manager)
            .Include(e => e.EmployeeDepartments)
                .ThenInclude(ed => ed.Department)
            .Include(e => e.ManagedDepartments)
                .ThenInclude(dm => dm.Department)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(string id)
    {
        return await _context.Employees
            .Include(e => e.Manager)
            .Include(e => e.EmployeeDepartments)
                .ThenInclude(ed => ed.Department)
            .Include(e => e.ManagedDepartments)
                .ThenInclude(dm => dm.Department)
            .Include(e => e.DirectReports)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Employee> CreateAsync(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee> UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task DeleteAsync(string id)
    {
        var employee = await _context.Employees.FindAsync(id);
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
