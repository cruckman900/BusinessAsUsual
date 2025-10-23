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
            try
            {
                return await _db.Employees
                    .Where(e => e.CompanyId == CompanyId)
                    .OrderBy(e => e.LastName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                System.Diagnostics.Debug.WriteLine($"An error occurred while retrieving employees. {ex}");
                return new List<Employee>();
            }
        }

        /// <summary>
        /// Retrieves a single employee by their ID.
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public async Task<Employee?> GetEmployeeByIdAsync(Guid employeeId)
        {
            try
            {
                return await _db.Employees.FindAsync(employeeId);
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                System.Diagnostics.Debug.WriteLine($"An error occurred while retrieving the employee. {ex}");
                return null;
            }
        }

        /// <summary>
        /// Adds a new employee to the database.
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public async Task AddEmployeeAsync(Employee employee)
        {
            try
            {
                employee.CreatedAt = DateTime.UtcNow;
                employee.UpdatedAt = DateTime.UtcNow;
                _db.Employees.Add(employee);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                System.Diagnostics.Debug.WriteLine($"An error occurred while adding the employee. {ex}");
            }
        }

        /// <summary>
        /// Updates an existing employee in the database.
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public async Task UpdateEmployeeAsync(Employee employee)
        {
            try
            {
                employee.UpdatedAt = DateTime.UtcNow;
                _db.Employees.Update(employee);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                System.Diagnostics.Debug.WriteLine($"An error occurred while updating the employee. {ex}");
            }
        }

        /// <summary>
        /// Assigns a role to an employee.
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task AssignRoleAsync(Guid employeeId, string role)
        {
            try
            {
                var employee = await _db.Employees.FindAsync(employeeId);
                if (employee == null) return;

                employee.Role = role;
                employee.UpdatedAt = DateTime.UtcNow;
                _db.Employees.Update(employee);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                System.Diagnostics.Debug.WriteLine($"An error occurred while assigning role to the employee. {ex}");
            }
        }

        /// <summary>
        /// Toggles the active status of an employee.
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public async Task ToggleStatusAsync(Guid employeeId)
        {
            try
            {
                var employee = await _db.Employees.FindAsync(employeeId);
                if (employee == null) return;

                employee.IsActive = !employee.IsActive;
                employee.UpdatedAt = DateTime.UtcNow;
                _db.Employees.Update(employee);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                System.Diagnostics.Debug.WriteLine($"An error occurred while toggling the employee status. {ex}");
            }
        }
    }
}
