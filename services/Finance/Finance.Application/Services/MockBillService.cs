using Finance.Application.DTOs;
using Finance.Domain.Enums;

namespace Finance.Application.Services;

public class MockBillService : IBillService
{
    private readonly List<BillDto> _bills = new();
    private int _billCounter = 1;

    public MockBillService()
    {
        // Seed with sample data
        _bills.Add(new BillDto
        {
            Id = Guid.NewGuid().ToString(),
            BillNumber = "BILL-001",
            VendorName = "Office Supplies Co",
            VendorEmail = "billing@officesupplies.com",
            Status = BillStatus.Received.ToString(),
            Currency = Currency.USD.ToString(),
            BillDate = DateTime.Now.AddDays(-5),
            DueDate = DateTime.Now.AddDays(25),
            Terms = "Net 30",
            LineItems = new List<BillLineItemDto>
            {
                new BillLineItemDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Description = "Office chairs (10 units)",
                    Quantity = 10,
                    UnitPrice = 150m,
                    TaxPercent = 8m,
                    ExpenseCategory = "Office Equipment"
                },
                new BillLineItemDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Description = "Desks (5 units)",
                    Quantity = 5,
                    UnitPrice = 300m,
                    TaxPercent = 8m,
                    ExpenseCategory = "Office Equipment"
                }
            },
            CreatedDate = DateTime.Now.AddDays(-5)
        });

        _bills.Add(new BillDto
        {
            Id = Guid.NewGuid().ToString(),
            BillNumber = "BILL-002",
            VendorName = "Tech Solutions Inc",
            VendorEmail = "accounts@techsolutions.com",
            Status = BillStatus.Paid.ToString(),
            Currency = Currency.USD.ToString(),
            BillDate = DateTime.Now.AddDays(-30),
            DueDate = DateTime.Now.AddDays(-5),
            PaidDate = DateTime.Now.AddDays(-7),
            Terms = "Net 30",
            LineItems = new List<BillLineItemDto>
            {
                new BillLineItemDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Description = "Annual software license",
                    Quantity = 1,
                    UnitPrice = 5000m,
                    TaxPercent = 0m,
                    ExpenseCategory = "Software"
                }
            },
            CreatedDate = DateTime.Now.AddDays(-30)
        });

        // Calculate totals for sample bills
        foreach (var bill in _bills)
        {
            CalculateTotals(bill);
        }

        _billCounter = 3;
    }

    public Task<IEnumerable<BillDto>> GetAllBillsAsync()
        => Task.FromResult<IEnumerable<BillDto>>(_bills);

    public Task<BillDto?> GetBillByIdAsync(string id)
        => Task.FromResult(_bills.FirstOrDefault(b => b.Id == id));

    public Task<IEnumerable<BillDto>> GetBillsByVendorAsync(string vendorId)
        => Task.FromResult<IEnumerable<BillDto>>(_bills.Where(b => b.VendorId == vendorId));

    public Task<BillDto> CreateBillAsync(CreateBillRequest request)
    {
        var bill = new BillDto
        {
            Id = Guid.NewGuid().ToString(),
            BillNumber = $"BILL-{_billCounter++:D3}",
            VendorId = request.VendorId,
            VendorName = request.VendorName,
            VendorEmail = request.VendorEmail,
            Status = BillStatus.Draft.ToString(),
            Currency = request.Currency.ToString(),
            BillDate = DateTime.Now,
            DueDate = request.DueDate,
            Notes = request.Notes,
            Terms = request.Terms,
            AssignedToEmployeeId = request.AssignedToEmployeeId,
            LineItems = request.LineItems.Select(li => new BillLineItemDto
            {
                Id = Guid.NewGuid().ToString(),
                Description = li.Description,
                Quantity = li.Quantity,
                UnitPrice = li.UnitPrice,
                TaxPercent = li.TaxPercent,
                ExpenseCategory = li.ExpenseCategory
            }).ToList(),
            Tags = request.Tags,
            CreatedDate = DateTime.Now
        };

        CalculateTotals(bill);
        _bills.Add(bill);

        return Task.FromResult(bill);
    }

    public Task<BillDto> UpdateBillAsync(string id, UpdateBillRequest request)
    {
        var bill = _bills.FirstOrDefault(b => b.Id == id);
        if (bill is null)
            throw new KeyNotFoundException($"Bill {id} not found");

        bill.VendorId = request.VendorId;
        bill.VendorName = request.VendorName;
        bill.VendorEmail = request.VendorEmail;
        bill.Status = request.Status.ToString();
        bill.Currency = request.Currency.ToString();
        bill.DueDate = request.DueDate;
        bill.Notes = request.Notes;
        bill.Terms = request.Terms;
        bill.AssignedToEmployeeId = request.AssignedToEmployeeId;
        bill.LineItems = request.LineItems.Select(li => new BillLineItemDto
        {
            Id = Guid.NewGuid().ToString(),
            Description = li.Description,
            Quantity = li.Quantity,
            UnitPrice = li.UnitPrice,
            TaxPercent = li.TaxPercent,
            ExpenseCategory = li.ExpenseCategory
        }).ToList();
        bill.Tags = request.Tags;

        if (request.Status == BillStatus.Paid && bill.PaidDate == null)
            bill.PaidDate = DateTime.Now;

        CalculateTotals(bill);

        return Task.FromResult(bill);
    }

    public Task DeleteBillAsync(string id)
    {
        var bill = _bills.FirstOrDefault(b => b.Id == id);
        if (bill is not null)
            _bills.Remove(bill);

        return Task.CompletedTask;
    }

    private static void CalculateTotals(BillDto bill)
    {
        bill.Subtotal = bill.LineItems.Sum(li => li.Quantity * li.UnitPrice);
        bill.TotalTax = bill.LineItems.Sum(li =>
        {
            var lineSubtotal = li.Quantity * li.UnitPrice;
            return lineSubtotal * (li.TaxPercent / 100m);
        });
        bill.Total = bill.Subtotal + bill.TotalTax;

        foreach (var item in bill.LineItems)
        {
            var lineSubtotal = item.Quantity * item.UnitPrice;
            var lineTax = lineSubtotal * (item.TaxPercent / 100m);
            item.LineTotal = lineSubtotal + lineTax;
        }

        bill.AmountPaid = bill.Status == BillStatus.Paid.ToString() ? bill.Total : 0m;
        bill.BalanceDue = bill.Total - bill.AmountPaid;
        bill.IsPaid = bill.Status == BillStatus.Paid.ToString();
        bill.IsOverdue = bill.DueDate.HasValue && bill.DueDate < DateTime.Now && !bill.IsPaid;
    }
}
