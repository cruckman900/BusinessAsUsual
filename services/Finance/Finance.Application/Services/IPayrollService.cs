using Finance.Application.DTOs;

namespace Finance.Application.Services;

/// <summary>
/// Finance payroll service. Ingests timesheets submitted by HR (holding them as
/// pending payroll) and runs payroll on demand, rolling all unprocessed
/// timesheets into a pay run. The goal of the HR -> Finance flow is that
/// Finance only has to "run payroll".
/// </summary>
public interface IPayrollService
{
    /// <summary>Ingest a timesheet submitted by HR. Idempotent by source timesheet id.</summary>
    Task IngestTimesheetAsync(
        string sourceTimesheetId,
        string employeeId,
        string employeeName,
        DateTime workDate,
        decimal totalWorkedHours,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<PendingTimesheetDto>> GetPendingTimesheetsAsync();

    Task<IEnumerable<PayRunDto>> GetPayRunsAsync();

    Task<PayRunDto?> GetPayRunByIdAsync(string id);

    /// <summary>Roll all unprocessed timesheets into a new completed pay run.</summary>
    Task<PayRunDto> RunPayrollAsync(CancellationToken cancellationToken = default);
}
