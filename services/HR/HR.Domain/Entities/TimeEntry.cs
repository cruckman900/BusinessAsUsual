namespace HR.Domain.Entities;

/// <summary>
/// A single time-clock punch for an employee. Entries pair up (clock-in then
/// clock-out) to form worked intervals; breaks and lunch are recorded as the
/// clock-out reason so they can be excluded from paid time.
/// </summary>
public class TimeEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;

    /// <summary>Local work date this entry belongs to (date component only).</summary>
    public DateTime WorkDate { get; set; }

    /// <summary>Reason/action, e.g. "start-day", "break", "lunch", "return", "end-day", "extra".</summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>True when this punch opens a working interval; false when it closes one.</summary>
    public bool IsClockIn { get; set; }

    public DateTime PunchedAtUtc { get; set; } = DateTime.UtcNow;
}
