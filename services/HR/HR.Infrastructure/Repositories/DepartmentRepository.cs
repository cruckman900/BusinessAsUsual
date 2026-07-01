using HR.Domain.Entities;
using HR.Domain.Repositories;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Repositories;

/// <summary>
/// Department repository implementation
/// </summary>
public class DepartmentRepository : IDepartmentRepository
{
    private readonly HRDbContext _context;

    public DepartmentRepository(HRDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Department>> GetAllAsync()
    {
#pragma warning disable CS0618 // Type or member is obsolete - loading legacy Manager navigation for backward compatibility
        return await _context.Departments
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

    public async Task<Department?> GetByIdAsync(string id)
    {
#pragma warning disable CS0618 // Type or member is obsolete - loading legacy Manager navigation for backward compatibility
        return await _context.Departments
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
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
        return department;
    }

    public async Task<Department> UpdateAsync(Department department)
    {
        _context.Departments.Update(department);
        await _context.SaveChangesAsync();
        return department;
    }

    public async Task DeleteAsync(string id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department != null)
        {
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
        }
    }
}
