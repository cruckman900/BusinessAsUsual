using Finance.Application.DTOs;
using Finance.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Finance.Application.Services;

/// <summary>
/// Holds timesheets received from HR as pending payroll and produces pay runs
/// on demand. Payroll math is intentionally simple (hours * hourly rate) so the
/// end-to-end flow works today; deductions/taxes can layer on later.
/// </summary>
public sealed class PayrollService : IPayrollService
{
    private readonly PayrollDataStore _store;
    private readonly ILogger<PayrollService> _logger;

    public PayrollService(PayrollDataStore store, ILogger<PayrollService> logger)
    {
        _store = store;
        _logger = logger;
    }

    public Task IngestTimesheetAsync(
        string sourceTimesheetId,
        string employeeId,
        string employeeName,
        DateTime workDate,
        decimal totalWorkedHours,
        CancellationToken cancellationToken = default)
    {
        var received = new ReceivedTimesheet
        {
            SourceTimesheetId = sourceTimesheetId,
            EmployeeId = employeeId,
            EmployeeName = employeeName,
            WorkDate = workDate,
            TotalWorkedHours = totalWorkedHours
        };

        // Idempotent: ignore duplicate deliveries of the same HR timesheet.
        var added = _store.PendingTimesheets.TryAdd(sourceTimesheetId, received);
        if (added)
        {
            _logger.LogInformation(
                "Received timesheet {SourceTimesheetId} for employee {EmployeeId} ({Hours}h) - pending payroll",
                sourceTimesheetId, employeeId, totalWorkedHours);
        }
        else
        {
            _logger.LogDebug("Duplicate timesheet {SourceTimesheetId} ignored", sourceTimesheetId);
        }

        return Task.CompletedTask;
    }

    public Task<IEnumerable<PendingTimesheetDto>> GetPendingTimesheetsAsync()
    {
        var result = _store.PendingTimesheets.Values
            .OrderByDescending(t => t.WorkDate)
            .Select(MapPending)
            .ToList();
        return Task.FromResult<IEnumerable<PendingTimesheetDto>>(result);
    }

    public Task<IEnumerable<PayRunDto>> GetPayRunsAsync()
    {
        var result = _store.PayRuns.Values
            .OrderByDescending(r => r.RunDateUtc)
            .Select(MapPayRun)
            .ToList();
        return Task.FromResult<IEnumerable<PayRunDto>>(result);
    }

    public Task<PayRunDto?> GetPayRunByIdAsync(string id)
    {
        return Task.FromResult(_store.PayRuns.TryGetValue(id, out var run)
            ? MapPayRun(run)
            : null);
    }

    public Task<PayRunDto> RunPayrollAsync(CancellationToken cancellationToken = default)
    {
        var unprocessed = _store.PendingTimesheets.Values
            .Where(t => !t.IsProcessed)
            .ToList();

        var payRun = new PayRun
        {
            Status = PayRunStatus.Completed,
            PeriodStart = unprocessed.Count > 0 ? unprocessed.Min(t => t.WorkDate) : null,
            PeriodEnd = unprocessed.Count > 0 ? unprocessed.Max(t => t.WorkDate) : null
        };

        foreach (var group in unprocessed.GroupBy(t => t.EmployeeId))
        {
            var hours = group.Sum(t => t.TotalWorkedHours);
            var rate = _store.DefaultHourlyRate;

            payRun.Lines.Add(new PayRunLine
            {
                EmployeeId = group.Key,
                EmployeeName = group.First().EmployeeName,
                TotalHours = hours,
                HourlyRate = rate,
                GrossPay = Math.Round(hours * rate, 2),
                TimesheetIds = group.Select(t => t.SourceTimesheetId).ToList()
            });

            foreach (var ts in group)
            {
                ts.IsProcessed = true;
                ts.PayRunId = payRun.Id;
            }
        }

        _store.PayRuns[payRun.Id] = payRun;

        _logger.LogInformation(
            "Ran payroll {PayRunId}: {Employees} employees, {Hours}h, {Gross:C} gross",
            payRun.Id, payRun.EmployeeCount, payRun.TotalHours, payRun.TotalGrossPay);

        return Task.FromResult(MapPayRun(payRun));
    }

    private static PendingTimesheetDto MapPending(ReceivedTimesheet t) => new()
    {
        Id = t.Id,
        SourceTimesheetId = t.SourceTimesheetId,
        EmployeeId = t.EmployeeId,
        EmployeeName = t.EmployeeName,
        WorkDate = t.WorkDate,
        TotalWorkedHours = t.TotalWorkedHours,
        ReceivedAtUtc = t.ReceivedAtUtc,
        IsProcessed = t.IsProcessed,
        PayRunId = t.PayRunId
    };

    private static PayRunDto MapPayRun(PayRun run) => new()
    {
        Id = run.Id,
        RunDateUtc = run.RunDateUtc,
        PeriodStart = run.PeriodStart,
        PeriodEnd = run.PeriodEnd,
        Status = run.Status.ToString(),
        TotalHours = run.TotalHours,
        TotalGrossPay = run.TotalGrossPay,
        EmployeeCount = run.EmployeeCount,
        Lines = run.Lines.Select(l => new PayRunLineDto
        {
            Id = l.Id,
            EmployeeId = l.EmployeeId,
            EmployeeName = l.EmployeeName,
            TotalHours = l.TotalHours,
            HourlyRate = l.HourlyRate,
            GrossPay = l.GrossPay,
            TimesheetIds = l.TimesheetIds
        }).ToList()
    };
}
