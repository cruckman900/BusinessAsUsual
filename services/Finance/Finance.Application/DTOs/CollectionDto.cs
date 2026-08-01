namespace Finance.Application.DTOs;

public class AgingBucketDto
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal Current { get; set; }
    public decimal Days30 { get; set; }
    public decimal Days60 { get; set; }
    public decimal Days90 { get; set; }
    public decimal Days120Plus { get; set; }
    public decimal TotalOverdue { get; set; }
    public int OverdueInvoiceCount { get; set; }
    public DateTime? OldestInvoiceDate { get; set; }
}

public class AgingReportDto
{
    public DateTime ReportDate { get; set; }
    public List<AgingBucketDto> Buckets { get; set; } = new();
    public decimal TotalCurrent { get; set; }
    public decimal Total30Days { get; set; }
    public decimal Total60Days { get; set; }
    public decimal Total90Days { get; set; }
    public decimal Total120Plus { get; set; }
    public decimal GrandTotal { get; set; }
}

public class CollectionActivityDto
{
    public string Id { get; set; } = string.Empty;
    public string InvoiceId { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public string? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty;
    public DateTime ActivityDate { get; set; }
    public string? ContactMethod { get; set; }
    public string? ContactPerson { get; set; }
    public string? Notes { get; set; }
    public string? Outcome { get; set; }
    public DateTime? PromisedPaymentDate { get; set; }
    public decimal? PromisedAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? AssignedToEmployeeId { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class CreateCollectionActivityRequest
{
    public string InvoiceId { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty;
    public string? ContactMethod { get; set; }
    public string? ContactPerson { get; set; }
    public string? Notes { get; set; }
    public string? Outcome { get; set; }
    public DateTime? PromisedPaymentDate { get; set; }
    public decimal? PromisedAmount { get; set; }
    public string? AssignedToEmployeeId { get; set; }
}

public class UpdateCollectionActivityRequest
{
    public string ActivityType { get; set; } = string.Empty;
    public string? ContactMethod { get; set; }
    public string? ContactPerson { get; set; }
    public string? Notes { get; set; }
    public string? Outcome { get; set; }
    public DateTime? PromisedPaymentDate { get; set; }
    public decimal? PromisedAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}
