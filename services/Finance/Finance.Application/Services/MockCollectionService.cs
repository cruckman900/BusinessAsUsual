using Finance.Application.DTOs;

namespace Finance.Application.Services;

public class MockCollectionService : ICollectionService
{
    private readonly IInvoiceService _invoiceService;
    private readonly List<CollectionActivityDto> _activities = new();

    public MockCollectionService(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;

        // Seed sample activities
        _activities.Add(new CollectionActivityDto
        {
            Id = Guid.NewGuid().ToString(),
            InvoiceId = "sample-invoice-1",
            InvoiceNumber = "INV-001",
            CustomerName = "Acme Corp",
            ActivityType = "Phone Call",
            ActivityDate = DateTime.Now.AddDays(-5),
            ContactMethod = "Phone",
            ContactPerson = "John Smith",
            Notes = "Spoke with accounts payable. Payment expected by end of week.",
            Outcome = "Payment Promised",
            PromisedPaymentDate = DateTime.Now.AddDays(2),
            PromisedAmount = 5000m,
            Status = "Open",
            CreatedDate = DateTime.Now.AddDays(-5)
        });

        _activities.Add(new CollectionActivityDto
        {
            Id = Guid.NewGuid().ToString(),
            InvoiceId = "sample-invoice-2",
            InvoiceNumber = "INV-002",
            CustomerName = "TechStart Inc",
            ActivityType = "Email",
            ActivityDate = DateTime.Now.AddDays(-10),
            ContactMethod = "Email",
            Notes = "Sent payment reminder. No response yet.",
            Outcome = "No Response",
            Status = "Open",
            CreatedDate = DateTime.Now.AddDays(-10)
        });
    }

    public async Task<AgingReportDto> GenerateAgingReportAsync(DateTime? asOfDate = null)
    {
        var reportDate = asOfDate ?? DateTime.Now;
        var invoices = await _invoiceService.GetAllInvoicesAsync();

        // Group by customer and calculate aging buckets
        var customerGroups = invoices
            .Where(inv => inv.BalanceDue > 0)
            .GroupBy(inv => new { inv.CustomerId, inv.CustomerName });

        var buckets = new List<AgingBucketDto>();

        foreach (var group in customerGroups)
        {
            var bucket = new AgingBucketDto
            {
                CustomerId = group.Key.CustomerId ?? "",
                CustomerName = group.Key.CustomerName
            };

            foreach (var invoice in group)
            {
                if (!invoice.DueDate.HasValue) continue;

                var daysOverdue = (reportDate - invoice.DueDate.Value).Days;
                var balance = invoice.BalanceDue;

                if (daysOverdue < 0)
                    bucket.Current += balance;
                else if (daysOverdue <= 30)
                    bucket.Days30 += balance;
                else if (daysOverdue <= 60)
                    bucket.Days60 += balance;
                else if (daysOverdue <= 90)
                    bucket.Days90 += balance;
                else
                    bucket.Days120Plus += balance;

                bucket.TotalOverdue += balance;
                bucket.OverdueInvoiceCount++;

                if (bucket.OldestInvoiceDate == null || invoice.IssueDate < bucket.OldestInvoiceDate)
                    bucket.OldestInvoiceDate = invoice.IssueDate;
            }

            buckets.Add(bucket);
        }

        var report = new AgingReportDto
        {
            ReportDate = reportDate,
            Buckets = buckets.OrderByDescending(b => b.TotalOverdue).ToList(),
            TotalCurrent = buckets.Sum(b => b.Current),
            Total30Days = buckets.Sum(b => b.Days30),
            Total60Days = buckets.Sum(b => b.Days60),
            Total90Days = buckets.Sum(b => b.Days90),
            Total120Plus = buckets.Sum(b => b.Days120Plus)
        };

        report.GrandTotal = report.TotalCurrent + report.Total30Days + report.Total60Days + 
                           report.Total90Days + report.Total120Plus;

        return report;
    }

    public Task<IEnumerable<CollectionActivityDto>> GetAllActivitiesAsync()
        => Task.FromResult<IEnumerable<CollectionActivityDto>>(_activities.OrderByDescending(a => a.ActivityDate));

    public Task<IEnumerable<CollectionActivityDto>> GetActivitiesByInvoiceAsync(string invoiceId)
        => Task.FromResult<IEnumerable<CollectionActivityDto>>(
            _activities.Where(a => a.InvoiceId == invoiceId).OrderByDescending(a => a.ActivityDate));

    public Task<IEnumerable<CollectionActivityDto>> GetActivitiesByCustomerAsync(string customerId)
        => Task.FromResult<IEnumerable<CollectionActivityDto>>(
            _activities.Where(a => a.CustomerId == customerId).OrderByDescending(a => a.ActivityDate));

    public Task<CollectionActivityDto?> GetActivityByIdAsync(string id)
        => Task.FromResult(_activities.FirstOrDefault(a => a.Id == id));

    public Task<CollectionActivityDto> CreateActivityAsync(CreateCollectionActivityRequest request)
    {
        var activity = new CollectionActivityDto
        {
            Id = Guid.NewGuid().ToString(),
            InvoiceId = request.InvoiceId,
            ActivityType = request.ActivityType,
            ActivityDate = DateTime.Now,
            ContactMethod = request.ContactMethod,
            ContactPerson = request.ContactPerson,
            Notes = request.Notes,
            Outcome = request.Outcome,
            PromisedPaymentDate = request.PromisedPaymentDate,
            PromisedAmount = request.PromisedAmount,
            Status = "Open",
            AssignedToEmployeeId = request.AssignedToEmployeeId,
            CreatedDate = DateTime.Now
        };

        _activities.Add(activity);
        return Task.FromResult(activity);
    }

    public Task<CollectionActivityDto> UpdateActivityAsync(string id, UpdateCollectionActivityRequest request)
    {
        var activity = _activities.FirstOrDefault(a => a.Id == id);
        if (activity == null)
            throw new KeyNotFoundException($"Collection activity {id} not found");

        activity.ActivityType = request.ActivityType;
        activity.ContactMethod = request.ContactMethod;
        activity.ContactPerson = request.ContactPerson;
        activity.Notes = request.Notes;
        activity.Outcome = request.Outcome;
        activity.PromisedPaymentDate = request.PromisedPaymentDate;
        activity.PromisedAmount = request.PromisedAmount;
        activity.Status = request.Status;

        return Task.FromResult(activity);
    }

    public Task DeleteActivityAsync(string id)
    {
        var activity = _activities.FirstOrDefault(a => a.Id == id);
        if (activity != null)
            _activities.Remove(activity);

        return Task.CompletedTask;
    }
}
