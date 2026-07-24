namespace Finance.Application.DTOs;

public class PendingTimesheetDto
{
    public string Id { get; set; } = string.Empty;
    public string SourceTimesheetId { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime WorkDate { get; set; }
    public decimal TotalWorkedHours { get; set; }
    public DateTime ReceivedAtUtc { get; set; }
    public bool IsProcessed { get; set; }
    public string? PayRunId { get; set; }
}

public class PayRunLineDto
{
    public string Id { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public decimal TotalHours { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal GrossPay { get; set; }
    public List<string> TimesheetIds { get; set; } = new();
}

public class PayRunDto
{
    public string Id { get; set; } = string.Empty;
    public DateTime RunDateUtc { get; set; }
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalHours { get; set; }
    public decimal TotalGrossPay { get; set; }
    public int EmployeeCount { get; set; }
    public List<PayRunLineDto> Lines { get; set; } = new();
}
