namespace HR.Domain.Entities;

/// <summary>
/// Daily aggregate of an employee's <see cref="TimeEntry"/> punches. Produced
/// by HR when the employee clocks out for the day and handed to Finance for
/// payroll.
/// </summary>
public class Timesheet
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;

    /// <summary>Local work date the timesheet covers (date component only).</summary>
    public DateTime WorkDate { get; set; }

    /// <summary>Total worked hours (excludes breaks and lunch).</summary>
    public decimal TotalWorkedHours { get; set; }

    public TimesheetStatus Status { get; set; } = TimesheetStatus.Open;

    public DateTime? SubmittedAtUtc { get; set; }

    public List<TimeEntry> Entries { get; set; } = new();
}

public enum TimesheetStatus
{
    Open,
    Submitted
}
