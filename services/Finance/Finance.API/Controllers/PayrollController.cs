using Finance.Application.DTOs;
using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers;

/// <summary>
/// Payroll endpoints. HR-submitted timesheets flow in via the event bus and are
/// held here as pending payroll; at the end of the period Finance only has to
/// "run payroll".
/// </summary>
[ApiController]
[Route("api/finance/payroll")]
public class PayrollController : ControllerBase
{
    private readonly IPayrollService _payrollService;
    private readonly ILogger<PayrollController> _logger;

    public PayrollController(IPayrollService payrollService, ILogger<PayrollController> logger)
    {
        _payrollService = payrollService;
        _logger = logger;
    }

    /// <summary>Timesheets received from HR that are pending payroll.</summary>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(IEnumerable<PendingTimesheetDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PendingTimesheetDto>>> GetPending()
    {
        var pending = await _payrollService.GetPendingTimesheetsAsync();
        return Ok(pending);
    }

    /// <summary>All completed pay runs.</summary>
    [HttpGet("pay-runs")]
    [ProducesResponseType(typeof(IEnumerable<PayRunDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PayRunDto>>> GetPayRuns()
    {
        var runs = await _payrollService.GetPayRunsAsync();
        return Ok(runs);
    }

    /// <summary>A single pay run by id.</summary>
    [HttpGet("pay-runs/{id}")]
    [ProducesResponseType(typeof(PayRunDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PayRunDto>> GetPayRun(string id)
    {
        var run = await _payrollService.GetPayRunByIdAsync(id);
        if (run == null)
        {
            return NotFound(new { message = $"Pay run with ID {id} not found" });
        }

        return Ok(run);
    }

    /// <summary>Run payroll, rolling all pending timesheets into a new pay run.</summary>
    [HttpPost("run")]
    [ProducesResponseType(typeof(PayRunDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PayRunDto>> RunPayroll()
    {
        _logger.LogInformation("Running payroll");
        var run = await _payrollService.RunPayrollAsync();
        return Ok(run);
    }
}
