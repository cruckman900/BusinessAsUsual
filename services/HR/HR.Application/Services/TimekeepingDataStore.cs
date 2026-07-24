using System.Collections.Concurrent;
using HR.Domain.Entities;

namespace HR.Application.Services;

/// <summary>
/// In-memory store for HR timekeeping. Structured so it can be swapped for an
/// EF-backed repository later without changing the service surface. Singleton so
/// punches accumulate across requests within the HR host.
/// </summary>
public sealed class TimekeepingDataStore
{
    // Keyed by timesheet id.
    public ConcurrentDictionary<string, Timesheet> Timesheets { get; } = new();

    /// <summary>
    /// Finds the open timesheet for an employee on a given work date, or creates
    /// one if none exists yet.
    /// </summary>
    public Timesheet GetOrCreateOpenTimesheet(string employeeId, string employeeName, DateTime workDate)
    {
        var existing = Timesheets.Values.FirstOrDefault(t =>
            t.EmployeeId == employeeId &&
            t.WorkDate.Date == workDate.Date &&
            t.Status == TimesheetStatus.Open);

        if (existing is not null)
        {
            return existing;
        }

        var sheet = new Timesheet
        {
            EmployeeId = employeeId,
            EmployeeName = employeeName,
            WorkDate = workDate.Date,
            Status = TimesheetStatus.Open
        };
        Timesheets[sheet.Id] = sheet;
        return sheet;
    }
}
