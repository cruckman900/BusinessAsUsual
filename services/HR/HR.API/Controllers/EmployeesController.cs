using HR.Application.DTOs;
using HR.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;

/// <summary>
/// Employee management endpoints
/// </summary>
[ApiController]
[Route("api/hr/employees")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
    {
        _employeeService = employeeService;
        _logger = logger;
    }

    /// <summary>
    /// Get all employees
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
    {
        _logger.LogInformation("Getting all employees");
        var employees = await _employeeService.GetAllEmployeesAsync();
        return Ok(employees);
    }

    /// <summary>
    /// Get employee by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(string id)
    {
        _logger.LogInformation("Getting employee {EmployeeId}", id);
        var employee = await _employeeService.GetEmployeeByIdAsync(id);

        if (employee == null)
        {
            return NotFound(new { message = $"Employee with ID {id} not found" });
        }

        return Ok(employee);
    }

    /// <summary>
    /// Create new employee
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] CreateEmployeeRequest request)
    {
        _logger.LogInformation("Creating new employee: {FirstName} {LastName}", request.FirstName, request.LastName);

        var employee = await _employeeService.CreateEmployeeAsync(request);
        return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
    }

    /// <summary>
    /// Update employee
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeDto>> UpdateEmployee(string id, [FromBody] UpdateEmployeeRequest request)
    {
        _logger.LogInformation("Updating employee {EmployeeId}", id);

        try
        {
            var updated = await _employeeService.UpdateEmployeeAsync(id, request);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete employee
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEmployee(string id)
    {
        _logger.LogInformation("Deleting employee {EmployeeId}", id);

        await _employeeService.DeleteEmployeeAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Search employees
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> SearchEmployees([FromQuery] string q)
    {
        _logger.LogInformation("Searching employees with term: {SearchTerm}", q);

        var employees = await _employeeService.SearchEmployeesAsync(q);
        return Ok(employees);
    }
}
