namespace Finance.Domain.Entities;

/// <summary>
/// A payroll run. When Finance "runs payroll" all pending timesheets are rolled
/// up per employee into pay run lines. Gross pay uses a simple hourly rate for
/// now; deductions/taxes can layer on later.
/// </summary>
public class PayRun
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public DateTime RunDateUtc { get; set; } = DateTime.UtcNow;

    /// <summary>Inclusive period the run covers (earliest/latest work date consumed).</summary>
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }

    public PayRunStatus Status { get; set; } = PayRunStatus.Completed;

    public List<PayRunLine> Lines { get; set; } = new();

    public decimal TotalHours => Lines.Sum(l => l.TotalHours);
    public decimal TotalGrossPay => Lines.Sum(l => l.GrossPay);
    public decimal TotalTaxes => Lines.Sum(l => l.Taxes);
    public decimal TotalDeductions => Lines.Sum(l => l.Deductions);
    public decimal TotalNetPay => Lines.Sum(l => l.NetPay);
    public int EmployeeCount => Lines.Count;
}

public class PayRunLine
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;

    public decimal TotalHours { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal GrossPay { get; set; }

    /// <summary>Tax withheld from gross pay.</summary>
    public decimal Taxes { get; set; }

    /// <summary>Other deductions (benefits, etc.) taken from gross pay.</summary>
    public decimal Deductions { get; set; }

    /// <summary>Take-home pay after taxes and deductions.</summary>
    public decimal NetPay { get; set; }

    /// <summary>The received timesheet ids that were rolled into this line.</summary>
    public List<string> TimesheetIds { get; set; } = new();
}

public enum PayRunStatus
{
    Draft,
    Completed
}
