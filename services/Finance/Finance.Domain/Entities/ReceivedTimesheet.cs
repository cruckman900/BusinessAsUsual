namespace Finance.Domain.Entities;

/// <summary>
/// A timesheet received from HR (via the TimesheetSubmitted integration event)
/// that is pending payroll. Finance accumulates these until someone runs
/// payroll for the period.
/// </summary>
public class ReceivedTimesheet
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>The HR timesheet id this record originated from (idempotency key).</summary>
    public string SourceTimesheetId { get; set; } = string.Empty;

    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;

    public DateTime WorkDate { get; set; }
    public decimal TotalWorkedHours { get; set; }

    public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>True once this timesheet has been included in a completed pay run.</summary>
    public bool IsProcessed { get; set; }

    /// <summary>The pay run that consumed this timesheet, if any.</summary>
    public string? PayRunId { get; set; }
}
