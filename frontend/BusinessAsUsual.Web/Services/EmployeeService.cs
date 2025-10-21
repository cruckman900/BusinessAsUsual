using BusinessAsUsual.Web.Data.Context;
using BusinessAsUsual.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessAsUsual.Web.Services
{
    /// <summary>
    /// Provides operations for managing employee records.
    /// </summary>
    public class EmployeeService
    {
        private readonly CompanyDbContext _db;

        /// <summary>
        /// Inititializes a new instance of the <see cref="EmployeeService"/> class.
        /// </summary>
        /// <param name="db"></param>
        public EmployeeService(CompanyDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Retrieves all employees for a given company.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Employee>> GetAllEmployeesAsync(Guid CompanyId)
        {
            return await _db.Employees
                .Where(e => e.CompanyId == CompanyId)
                .OrderBy(e => e.LastName)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a single employee by their ID.
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public async Task<Employee?> GetEmployeeByIdAsync(Guid employeeId)
        {
            return await _db.Employees.FindAsync(employeeId);
        }

        /// <summary>
        /// Adds a new employee to the database.
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public async Task AddEmployeeAsync(Employee employee)
        {
            employee.CreatedAt = DateTime.UtcNow;
            employee.UpdatedAt = DateTime.UtcNow;
            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing employee in the database.
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public async Task UpdateEmployeeAsync(Employee employee)
        {
            employee.UpdatedAt = DateTime.UtcNow;
            _db.Employees.Update(employee);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Assigns a role to an employee.
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task AssignRoleAsync(Guid employeeId, string role)
        {
            var employee = await _db.Employees.FindAsync(employeeId);
            if (employee == null) return;

            employee.Role = role;
            employee.UpdatedAt = DateTime.UtcNow;
            _db.Employees.Update(employee);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Toggles the active status of an employee.
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public async Task ToggleStatusAsync(Guid employeeId)
        {
            var employee = await _db.Employees.FindAsync(employeeId);
            if (employee == null) return;

            employee.IsActive = !employee.IsActive;
            employee.UpdatedAt = DateTime.UtcNow;
            _db.Employees.Update(employee);
            await _db.SaveChangesAsync();
        }
    }
}
