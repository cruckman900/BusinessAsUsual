using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Core.Events.Integration;
using Finance.Application.Services;
using Microsoft.Extensions.Logging;

namespace Finance.Application.Events;

/// <summary>
/// When HR submits a daily timesheet, Finance holds it as pending payroll.
/// Nothing is paid automatically - at the end of the period Finance only has to
/// "run payroll", which rolls all pending timesheets into a pay run.
/// </summary>
public sealed class TimesheetSubmittedHandler : IIntegrationEventHandler<TimesheetSubmittedIntegrationEvent>
{
    private readonly IPayrollService _payrollService;
    private readonly ILogger<TimesheetSubmittedHandler> _logger;

    public TimesheetSubmittedHandler(IPayrollService payrollService, ILogger<TimesheetSubmittedHandler> logger)
    {
        _payrollService = payrollService;
        _logger = logger;
    }

    public async Task HandleAsync(TimesheetSubmittedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        var workDate = DateTime.TryParse(@event.WorkDate, out var parsed) ? parsed : @event.SubmittedAtUtc.Date;

        await _payrollService.IngestTimesheetAsync(
            @event.TimesheetId,
            @event.EmployeeId,
            @event.EmployeeName,
            workDate,
            @event.TotalWorkedHours,
            cancellationToken);

        _logger.LogInformation(
            "Held timesheet {TimesheetId} for employee {EmployeeId} as pending payroll",
            @event.TimesheetId, @event.EmployeeId);
    }
}
