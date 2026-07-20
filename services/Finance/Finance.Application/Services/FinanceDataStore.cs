using Finance.Domain.Entities;

namespace Finance.Application.Services;

/// <summary>
/// In-memory data store shared by the mock Finance services so invoices,
/// payments, and reports stay consistent within a running process.
/// Mirrors the mock-first approach used by CRM before EF persistence is added.
/// </summary>
public class FinanceDataStore
{
    public List<Invoice> Invoices { get; } = new();
    public List<Payment> Payments { get; } = new();

    private int _invoiceSequence = 1000;

    public FinanceDataStore()
    {
        Seed();
    }

    public string NextInvoiceNumber() => $"INV-{++_invoiceSequence}";

    private void Seed()
    {
        var inv1 = new Invoice
        {
            InvoiceNumber = NextInvoiceNumber(),
            CustomerName = "Acme Corporation",
            CustomerEmail = "billing@acme.example",
            Status = Domain.Enums.InvoiceStatus.Sent,
            IssueDate = DateTime.UtcNow.AddDays(-20),
            DueDate = DateTime.UtcNow.AddDays(10),
            SentDate = DateTime.UtcNow.AddDays(-20),
            LineItems =
            {
                new InvoiceLineItem { Description = "Consulting services", Quantity = 40, UnitPrice = 150m, TaxPercent = 8m }
            }
        };
        foreach (var li in inv1.LineItems) li.InvoiceId = inv1.Id;

        var inv2 = new Invoice
        {
            InvoiceNumber = NextInvoiceNumber(),
            CustomerName = "Globex LLC",
            CustomerEmail = "ap@globex.example",
            Status = Domain.Enums.InvoiceStatus.Paid,
            IssueDate = DateTime.UtcNow.AddDays(-45),
            DueDate = DateTime.UtcNow.AddDays(-15),
            SentDate = DateTime.UtcNow.AddDays(-45),
            PaidDate = DateTime.UtcNow.AddDays(-18),
            LineItems =
            {
                new InvoiceLineItem { Description = "Annual license", Quantity = 1, UnitPrice = 12000m, TaxPercent = 8m }
            }
        };
        foreach (var li in inv2.LineItems) li.InvoiceId = inv2.Id;

        var inv3 = new Invoice
        {
            InvoiceNumber = NextInvoiceNumber(),
            CustomerName = "Initech",
            CustomerEmail = "accounts@initech.example",
            Status = Domain.Enums.InvoiceStatus.Overdue,
            IssueDate = DateTime.UtcNow.AddDays(-60),
            DueDate = DateTime.UtcNow.AddDays(-30),
            SentDate = DateTime.UtcNow.AddDays(-60),
            LineItems =
            {
                new InvoiceLineItem { Description = "Support retainer", Quantity = 6, UnitPrice = 500m, TaxPercent = 8m }
            }
        };
        foreach (var li in inv3.LineItems) li.InvoiceId = inv3.Id;

        Invoices.AddRange(new[] { inv1, inv2, inv3 });

        var pay = new Payment
        {
            InvoiceId = inv2.Id,
            CustomerId = inv2.CustomerId,
            Amount = inv2.Total,
            Method = Domain.Enums.PaymentMethod.BankTransfer,
            Status = Domain.Enums.PaymentStatus.Completed,
            PaymentDate = DateTime.UtcNow.AddDays(-18),
            ProcessedDate = DateTime.UtcNow.AddDays(-18),
            TransactionReference = "TXN-SEED-0001"
        };
        Payments.Add(pay);
        inv2.Payments.Add(pay);
    }
}
