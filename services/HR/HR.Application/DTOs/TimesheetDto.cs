namespace HR.Application.DTOs;

public class TimeEntryDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime WorkDate { get; set; }
    public string Action { get; set; } = string.Empty;
    public bool IsClockIn { get; set; }
    public DateTime PunchedAtUtc { get; set; }
}

public class TimesheetDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime WorkDate { get; set; }
    public decimal TotalWorkedHours { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? SubmittedAtUtc { get; set; }
    public List<TimeEntryDto> Entries { get; set; } = new();
}
