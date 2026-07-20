namespace Finance.Application.DTOs;

public class FinanceSummaryDto
{
    public decimal TotalOutstanding { get; set; }
    public decimal TotalPaidThisMonth { get; set; }
    public decimal TotalOverdue { get; set; }
    public int DraftCount { get; set; }
    public int SentCount { get; set; }
    public int PaidCount { get; set; }
    public int OverdueCount { get; set; }
    public List<RevenuePointDto> RevenueByMonth { get; set; } = new();
    public List<StatusBreakdownDto> StatusBreakdown { get; set; } = new();
}

public class RevenuePointDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class StatusBreakdownDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Amount { get; set; }
    public string Color { get; set; } = "#1976d2";
}
