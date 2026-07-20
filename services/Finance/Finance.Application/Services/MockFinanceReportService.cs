using Finance.Application.DTOs;
using Finance.Domain.Enums;

namespace Finance.Application.Services;

public class MockFinanceReportService : IFinanceReportService
{
    private readonly FinanceDataStore _store;

    public MockFinanceReportService(FinanceDataStore store) => _store = store;

    public Task<FinanceSummaryDto> GetSummaryAsync()
    {
        var invoices = _store.Invoices;
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        var summary = new FinanceSummaryDto
        {
            TotalOutstanding = invoices.Where(i => !i.IsPaid && i.Status != InvoiceStatus.Cancelled).Sum(i => i.BalanceDue),
            TotalOverdue = invoices.Where(i => i.IsOverdue).Sum(i => i.BalanceDue),
            TotalPaidThisMonth = _store.Payments
                .Where(p => p.Status == PaymentStatus.Completed && p.PaymentDate >= monthStart)
                .Sum(p => p.Amount),
            DraftCount = invoices.Count(i => i.Status == InvoiceStatus.Draft),
            SentCount = invoices.Count(i => i.Status == InvoiceStatus.Sent),
            PaidCount = invoices.Count(i => i.Status == InvoiceStatus.Paid),
            OverdueCount = invoices.Count(i => i.IsOverdue)
        };

        summary.RevenueByMonth = Enumerable.Range(0, 6)
            .Select(offset =>
            {
                var month = monthStart.AddMonths(-5 + offset);
                var next = month.AddMonths(1);
                var amount = _store.Payments
                    .Where(p => p.Status == PaymentStatus.Completed && p.PaymentDate >= month && p.PaymentDate < next)
                    .Sum(p => p.Amount);
                return new RevenuePointDto { Label = month.ToString("MMM yyyy"), Amount = amount };
            })
            .ToList();

        summary.StatusBreakdown = invoices
            .GroupBy(i => i.Status)
            .Select(g => new StatusBreakdownDto
            {
                Status = g.Key.ToString(),
                Count = g.Count(),
                Amount = g.Sum(i => i.Total),
                Color = ColorFor(g.Key)
            })
            .ToList();

        return Task.FromResult(summary);
    }

    private static string ColorFor(InvoiceStatus status) => status switch
    {
        InvoiceStatus.Draft => "#9e9e9e",
        InvoiceStatus.Sent => "#1976d2",
        InvoiceStatus.PartiallyPaid => "#ff9800",
        InvoiceStatus.Paid => "#4caf50",
        InvoiceStatus.Overdue => "#f44336",
        InvoiceStatus.Cancelled => "#607d8b",
        InvoiceStatus.Refunded => "#9c27b0",
        _ => "#1976d2"
    };
}
