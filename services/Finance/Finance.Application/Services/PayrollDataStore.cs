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

    /// <summary>Default hourly rate used until per-employee rates are wired in.</summary>
    public decimal DefaultHourlyRate { get; } = 25m;
}
