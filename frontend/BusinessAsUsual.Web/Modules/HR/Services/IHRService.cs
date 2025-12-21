using BusinessAsUsual.Web.Data.Entities;

namespace BusinessAsUsual.Web.Modules.HR.Services
{
    /// <summary>
    /// Defines operations for managing employees and benefits within a human resources system.
    /// </summary>
    /// <remarks>This interface provides asynchronous methods for retrieving employee and benefit information,
    /// as well as adding new employees. Implementations should ensure thread safety and handle data access exceptions
    /// appropriately. All methods are asynchronous and return tasks that complete when the operation
    /// finishes.</remarks>
    public interface IHRService
    {
        /// <summary>
        /// Asynchronously retrieves a collection of employees.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see
        /// cref="Employee"/> objects representing all employees. The collection will be empty if no employees are
        /// found.</returns>
        Task<IEnumerable<Employee>> GetEmployeesAsync();

        /// <summary>
        /// Asynchronously retrieves the employee record with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the employee to retrieve. Must be a positive integer.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Employee"/>
        /// instance if found; otherwise, <c>null</c>.</returns>
        Task<Employee?> GetEmployeeByIdAsync(int id);

        /// <summary>
        /// Asynchronously adds a new employee to the data store.
        /// </summary>
        /// <param name="employee">The employee to add. Cannot be null. All required properties of the employee must be set.</param>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        Task AddEmployeeAsync(Employee employee);

        /// <summary>
        /// Asynchronously retrieves a collection of available benefits.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see
        /// cref="Benefit"/> objects representing the available benefits. The collection will be empty if no benefits
        /// are available.</returns>
        Task<IEnumerable<Benefit>> GetBenefitsAsync();
    }

    // Simple models for now
    /// <summary>
    /// Represents an employee with identifying information and department assignment.
    /// </summary>
    /// <param name="Id">The unique identifier for the employee.</param>
    /// <param name="Name">The full name of the employee.</param>
    /// <param name="Department">The name of the department to which the employee is assigned.</param>
    /// <param name="Email">The email address of the employee.</param>
    public record Employee(int Id, string Name, string Department, string Email);

    /// <summary>
    /// Represents a benefit with a specified name and enabled state.
    /// </summary>
    /// <param name="Name">The name of the benefit. This value identifies the benefit and cannot be null.</param>
    /// <param name="Enabled">A value indicating whether the benefit is enabled. Specify <see langword="true"/> to enable the benefit;
    /// otherwise, <see langword="false"/>.</param>
    public record Benefit(string Name, bool Enabled);
}
