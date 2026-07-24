namespace HR.Application.DTOs;

/// <summary>
/// Request to record a single time-clock punch from the frontend clock widget.
/// </summary>
public class PunchRequest
{
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;

    /// <summary>Reason/action, e.g. "start-day", "break", "lunch", "return", "end-day", "extra".</summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>True when the punch opens a working interval; false when it closes one.</summary>
    public bool IsClockIn { get; set; }
}
