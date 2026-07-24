namespace BusinessAsUsual.Core.Events.Integration;

/// <summary>
/// Raised by HR whenever an employee punches the time clock (clock in/out for
/// the start of the day, breaks, lunch, end of day, or extra hours). HR's own
/// timekeeping consumer records the punch; when a day is closed out HR emits a
/// <see cref="TimesheetSubmittedIntegrationEvent"/> for Finance.
///
/// Cross-module contract, so it lives in BusinessAsUsual.Core where both the
/// publisher (HR) and any consumer can reference it.
/// </summary>
public sealed class TimeClockPunchedIntegrationEvent : IntegrationEvent
{
    public override string EventType => "hr.timeclock.punched";

    public string EmployeeId { get; init; } = string.Empty;
    public string EmployeeName { get; init; } = string.Empty;

    /// <summary>
    /// The reason/action for the punch, e.g. "start-day", "break", "lunch",
    /// "return", "end-day", "extra". Free-form but stable enough for routing.
    /// </summary>
    public string Action { get; init; } = string.Empty;

    /// <summary>True when the punch starts a working interval; false when it ends one.</summary>
    public bool IsClockIn { get; init; }

    /// <summary>Local work date this punch belongs to (yyyy-MM-dd).</summary>
    public string WorkDate { get; init; } = string.Empty;

    /// <summary>When the punch happened (UTC).</summary>
    public DateTime PunchedAtUtc { get; init; } = DateTime.UtcNow;
}
