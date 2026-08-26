using HR.Application.DTOs;

namespace HR.Application.Services;

/// <summary>
/// HR timekeeping service. Records time-clock punches, aggregates them into a
/// daily timesheet, and publishes integration events so downstream modules
/// (Finance) can react. On an end-of-day punch the timesheet is closed and a
/// TimesheetSubmitted event is published for payroll.
/// </summary>
public interface ITimekeepingService
{
    Task<TimesheetDto> RecordPunchAsync(PunchRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimesheetDto>> GetTimesheetsAsync(Guid? employeeId = null);
    Task<TimesheetDto?> GetTimesheetByIdAsync(Guid id);
}
