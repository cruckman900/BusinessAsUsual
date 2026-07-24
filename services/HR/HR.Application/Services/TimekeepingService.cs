using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Core.Events.Integration;
using HR.Application.DTOs;
using HR.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HR.Application.Services;

/// <summary>
/// Records time-clock punches into an open daily timesheet, publishing a
/// TimeClockPunched event for every punch. When the employee ends their day
/// (end-day punch) the timesheet is totaled, closed, and a TimesheetSubmitted
/// event is published so Finance can hold it as pending payroll.
/// </summary>
public sealed class TimekeepingService : ITimekeepingService
{
    private const string EndDayAction = "end-day";

    private readonly TimekeepingDataStore _store;
    private readonly IEventBus _eventBus;
    private readonly ILogger<TimekeepingService> _logger;

    public TimekeepingService(
        TimekeepingDataStore store,
        IEventBus eventBus,
        ILogger<TimekeepingService> logger)
    {
        _store = store;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<TimesheetDto> RecordPunchAsync(PunchRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var nowUtc = DateTime.UtcNow;
        var workDate = nowUtc.Date;

        var sheet = _store.GetOrCreateOpenTimesheet(request.EmployeeId, request.EmployeeName, workDate);

        var entry = new TimeEntry
        {
            EmployeeId = request.EmployeeId,
            EmployeeName = request.EmployeeName,
            WorkDate = workDate,
            Action = request.Action,
            IsClockIn = request.IsClockIn,
            PunchedAtUtc = nowUtc
        };
        sheet.Entries.Add(entry);
        sheet.TotalWorkedHours = CalculateWorkedHours(sheet);

        await _eventBus.PublishAsync(new TimeClockPunchedIntegrationEvent
        {
            EmployeeId = request.EmployeeId,
            EmployeeName = request.EmployeeName,
            Action = request.Action,
            IsClockIn = request.IsClockIn,
            WorkDate = workDate.ToString("yyyy-MM-dd"),
            PunchedAtUtc = nowUtc
        }, cancellationToken);

        _logger.LogInformation(
            "Recorded punch '{Action}' for employee {EmployeeId} on {WorkDate}",
            request.Action, request.EmployeeId, workDate.ToString("yyyy-MM-dd"));

        if (IsEndOfDay(request))
        {
            await SubmitTimesheetAsync(sheet, cancellationToken);
        }

        return MapToDto(sheet);
    }

    public Task<IEnumerable<TimesheetDto>> GetTimesheetsAsync(string? employeeId = null)
    {
        var query = _store.Timesheets.Values.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(employeeId))
        {
            query = query.Where(t => t.EmployeeId == employeeId);
        }

        var result = query
            .OrderByDescending(t => t.WorkDate)
            .Select(MapToDto)
            .ToList();

        return Task.FromResult<IEnumerable<TimesheetDto>>(result);
    }

    public Task<TimesheetDto?> GetTimesheetByIdAsync(string id)
    {
        return Task.FromResult(_store.Timesheets.TryGetValue(id, out var sheet)
            ? MapToDto(sheet)
            : null);
    }

    private async Task SubmitTimesheetAsync(Timesheet sheet, CancellationToken cancellationToken)
    {
        if (sheet.Status == TimesheetStatus.Submitted)
        {
            return;
        }

        sheet.TotalWorkedHours = CalculateWorkedHours(sheet);
        sheet.Status = TimesheetStatus.Submitted;
        sheet.SubmittedAtUtc = DateTime.UtcNow;

        await _eventBus.PublishAsync(new TimesheetSubmittedIntegrationEvent
        {
            TimesheetId = sheet.Id,
            EmployeeId = sheet.EmployeeId,
            EmployeeName = sheet.EmployeeName,
            WorkDate = sheet.WorkDate.ToString("yyyy-MM-dd"),
            TotalWorkedHours = sheet.TotalWorkedHours,
            SubmittedAtUtc = sheet.SubmittedAtUtc.Value
        }, cancellationToken);

        _logger.LogInformation(
            "Submitted timesheet {TimesheetId} for employee {EmployeeId}: {Hours} worked hours",
            sheet.Id, sheet.EmployeeId, sheet.TotalWorkedHours);
    }

    private static bool IsEndOfDay(PunchRequest request) =>
        !request.IsClockIn &&
        string.Equals(request.Action, EndDayAction, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Sums the duration of each clock-in followed by a clock-out. Breaks and
    /// lunch are recorded as clock-outs, so time spent on them is naturally
    /// excluded from the total.
    /// </summary>
    private static decimal CalculateWorkedHours(Timesheet sheet)
    {
        var ordered = sheet.Entries.OrderBy(e => e.PunchedAtUtc).ToList();
        var total = TimeSpan.Zero;
        DateTime? openedAt = null;

        foreach (var entry in ordered)
        {
            if (entry.IsClockIn)
            {
                openedAt ??= entry.PunchedAtUtc;
            }
            else if (openedAt is not null)
            {
                total += entry.PunchedAtUtc - openedAt.Value;
                openedAt = null;
            }
        }

        return Math.Round((decimal)total.TotalHours, 2);
    }

    private static TimesheetDto MapToDto(Timesheet sheet) => new()
    {
        Id = sheet.Id,
        EmployeeId = sheet.EmployeeId,
        EmployeeName = sheet.EmployeeName,
        WorkDate = sheet.WorkDate,
        TotalWorkedHours = sheet.TotalWorkedHours,
        Status = sheet.Status.ToString(),
        SubmittedAtUtc = sheet.SubmittedAtUtc,
        Entries = sheet.Entries
            .OrderBy(e => e.PunchedAtUtc)
            .Select(e => new TimeEntryDto
            {
                Id = e.Id,
                EmployeeId = e.EmployeeId,
                EmployeeName = e.EmployeeName,
                WorkDate = e.WorkDate,
                Action = e.Action,
                IsClockIn = e.IsClockIn,
                PunchedAtUtc = e.PunchedAtUtc
            })
            .ToList()
    };
}
