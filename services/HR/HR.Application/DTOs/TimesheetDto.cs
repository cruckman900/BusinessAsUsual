namespace HR.Application.DTOs;

public class TimeEntryDto
{
    public string Id { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime WorkDate { get; set; }
    public string Action { get; set; } = string.Empty;
    public bool IsClockIn { get; set; }
    public DateTime PunchedAtUtc { get; set; }
}

public class TimesheetDto
{
    public string Id { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime WorkDate { get; set; }
    public decimal TotalWorkedHours { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? SubmittedAtUtc { get; set; }
    public List<TimeEntryDto> Entries { get; set; } = new();
}
