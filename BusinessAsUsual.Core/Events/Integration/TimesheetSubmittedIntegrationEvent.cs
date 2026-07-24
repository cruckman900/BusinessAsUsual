namespace BusinessAsUsual.Core.Events.Integration;

/// <summary>
/// Raised by HR when an employee's daily timesheet is closed out (they clocked
/// out for the day). Finance consumes this to hold the timesheet as pending
/// payroll so that, at the end of the pay period, Finance only has to run
/// payroll.
///
/// Cross-module contract, so it lives in BusinessAsUsual.Core where both the
/// publisher (HR) and the consumer (Finance) can reference it.
/// </summary>
public sealed class TimesheetSubmittedIntegrationEvent : IntegrationEvent
{
    public override string EventType => "hr.timesheet.submitted";

    public string TimesheetId { get; init; } = string.Empty;
    public string EmployeeId { get; init; } = string.Empty;
    public string EmployeeName { get; init; } = string.Empty;

    /// <summary>Local work date the timesheet covers (yyyy-MM-dd).</summary>
    public string WorkDate { get; init; } = string.Empty;

    /// <summary>Total worked hours for the day (excludes breaks and lunch).</summary>
    public decimal TotalWorkedHours { get; init; }

    public DateTime SubmittedAtUtc { get; init; } = DateTime.UtcNow;
}
