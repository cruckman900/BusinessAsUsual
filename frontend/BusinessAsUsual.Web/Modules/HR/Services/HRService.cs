using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessAsUsual.Web.Modules.HR.Services
{
    /// <summary>
    /// Provides human resources operations for managing employees and their benefits.
    /// </summary>
    /// <remarks>The HRService class offers asynchronous methods to retrieve employee and benefit information,
    /// as well as to add new employees. This class is intended for use in applications that require basic HR data
    /// management functionality.</remarks>
    public class HRService : IHRService
    {
        private readonly List<Employee> _employees = new()
        {
            new Employee(1, "Alice Johnson", "Engineering", "alice@company.com"),
            new Employee(2, "Bob Smith", "HR", "bob@company.com")
        };

        private readonly List<Benefit> _benefits = new()
        {
            new Benefit("Health Insurance", true),
            new Benefit("Retirement Plan", true),
            new Benefit("Gym Membership", false),
        };

        /// <summary>
        /// Asynchronously retrieves a collection of all employees.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see
        /// cref="Employee"/> objects representing all employees. The collection will be empty if there are no
        /// employees.</returns>
        public Task<IEnumerable<Employee>> GetEmployeesAsync() => Task.FromResult(_employees.AsEnumerable());

        /// <summary>
        /// Asynchronously retrieves the employee with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the employee to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Employee"/> with
        /// the specified identifier, or <see langword="null"/> if no such employee exists.</returns>
        public Task<Employee?> GetEmployeeByIdAsync(int id) => Task.FromResult(_employees.FirstOrDefault(e  => e.Id == id));

        /// <summary>
        /// Asynchronously adds the specified employee to the collection.
        /// </summary>
        /// <param name="employee">The employee to add to the collection. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        public Task AddEmployeeAsync(Employee employee)
        {
            _employees.Add(employee);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously retrieves the collection of available benefits.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see
        /// cref="Benefit"/> objects representing the available benefits. The collection will be empty if no benefits
        /// are available.</returns>
        public Task<IEnumerable<Benefit>> GetBenefitsAsync() => Task.FromResult(_benefits.AsEnumerable());
    }
}
