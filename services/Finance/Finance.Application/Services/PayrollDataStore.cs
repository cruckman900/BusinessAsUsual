using System.Collections.Concurrent;
using Finance.Domain.Entities;

namespace Finance.Application.Services;

/// <summary>
/// In-memory store for payroll: timesheets received from HR that are pending
/// payroll, and completed pay runs. Singleton so state persists across requests
/// within the Finance host. Structured for a later EF-backed swap.
/// </summary>
public sealed class PayrollDataStore
{
    // Keyed by the HR source timesheet id for idempotent ingestion.
    public ConcurrentDictionary<string, ReceivedTimesheet> PendingTimesheets { get; } = new();

    public ConcurrentDictionary<string, PayRun> PayRuns { get; } = new();

    /// <summary>Default hourly rate used when an employee has no specific rate.</summary>
    public decimal DefaultHourlyRate { get; set; } = 25m;

    /// <summary>
    /// Per-employee hourly rates, keyed by EmployeeId. Falls back to
    /// <see cref="DefaultHourlyRate"/> when an employee is not listed. Seeded with
    /// a few examples (including the admin test account) until HR owns pay rates.
    /// </summary>
    public ConcurrentDictionary<string, decimal> EmployeeHourlyRates { get; } = new()
    {
        ["admin"] = 40m,
        ["EMP-0001"] = 30m,
    };

    /// <summary>Flat tax rate applied to gross pay (e.g. 0.20 = 20%). Placeholder for real tax tables.</summary>
    public decimal TaxRate { get; set; } = 0.20m;

    /// <summary>Flat other-deduction rate applied to gross pay (e.g. benefits) as a fraction.</summary>
    public decimal DeductionRate { get; set; } = 0.05m;

    /// <summary>Resolve an employee's hourly rate, falling back to the default.</summary>
    public decimal GetHourlyRate(string employeeId) =>
        EmployeeHourlyRates.TryGetValue(employeeId, out var rate) ? rate : DefaultHourlyRate;
}
