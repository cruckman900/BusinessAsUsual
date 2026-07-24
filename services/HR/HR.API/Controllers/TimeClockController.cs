using HR.Application.DTOs;
using HR.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;

/// <summary>
/// Time-clock endpoints. The frontend clock widget posts punches here; HR
/// records the timesheet and publishes integration events to drive the
/// HR -> Finance payroll flow.
/// </summary>
[ApiController]
[Route("api/hr/timeclock")]
public class TimeClockController : ControllerBase
{
    private readonly ITimekeepingService _timekeepingService;
    private readonly ILogger<TimeClockController> _logger;

    public TimeClockController(ITimekeepingService timekeepingService, ILogger<TimeClockController> logger)
    {
        _timekeepingService = timekeepingService;
        _logger = logger;
    }

    /// <summary>
    /// Record a time-clock punch (clock in/out for start of day, break, lunch,
    /// return, end of day, or extra hours).
    /// </summary>
    [HttpPost("punch")]
    [ProducesResponseType(typeof(TimesheetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TimesheetDto>> Punch([FromBody] PunchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EmployeeId))
        {
            return BadRequest(new { message = "EmployeeId is required" });
        }

        _logger.LogInformation("Punch '{Action}' for employee {EmployeeId}", request.Action, request.EmployeeId);
        var timesheet = await _timekeepingService.RecordPunchAsync(request);
        return Ok(timesheet);
    }

    /// <summary>
    /// Get timesheets, optionally filtered by employee.
    /// </summary>
    [HttpGet("timesheets")]
    [ProducesResponseType(typeof(IEnumerable<TimesheetDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TimesheetDto>>> GetTimesheets([FromQuery] string? employeeId = null)
    {
        var timesheets = await _timekeepingService.GetTimesheetsAsync(employeeId);
        return Ok(timesheets);
    }

    /// <summary>
    /// Get a single timesheet by id.
    /// </summary>
    [HttpGet("timesheets/{id}")]
    [ProducesResponseType(typeof(TimesheetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TimesheetDto>> GetTimesheet(string id)
    {
        var timesheet = await _timekeepingService.GetTimesheetByIdAsync(id);
        if (timesheet == null)
        {
            return NotFound(new { message = $"Timesheet with ID {id} not found" });
        }

        return Ok(timesheet);
    }
}
