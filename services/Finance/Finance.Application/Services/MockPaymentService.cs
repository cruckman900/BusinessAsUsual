using Finance.Application.DTOs;
using Finance.Domain.Entities;
using Finance.Domain.Enums;

namespace Finance.Application.Services;

public class MockPaymentService : IPaymentService
{
    private readonly FinanceDataStore _store;

    public MockPaymentService(FinanceDataStore store) => _store = store;

    public Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync()
        => Task.FromResult(_store.Payments.Select(p => p.ToDto(InvoiceNumberFor(p.InvoiceId))).AsEnumerable());

    public Task<PaymentDto?> GetPaymentByIdAsync(string id)
    {
        var p = _store.Payments.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(p?.ToDto(InvoiceNumberFor(p.InvoiceId)));
    }

    public Task<IEnumerable<PaymentDto>> GetPaymentsByInvoiceAsync(string invoiceId)
        => Task.FromResult(_store.Payments.Where(p => p.InvoiceId == invoiceId)
            .Select(p => p.ToDto(InvoiceNumberFor(p.InvoiceId))).AsEnumerable());

    public Task<PaymentDto> RecordPaymentAsync(RecordPaymentRequest request)
    {
        var payment = new Payment
        {
            InvoiceId = request.InvoiceId,
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            Currency = request.Currency,
            Method = request.Method,
            Status = request.Status,
            PaymentDate = request.PaymentDate ?? DateTime.UtcNow,
            ProcessedDate = request.Status == PaymentStatus.Completed ? DateTime.UtcNow : null,
            TransactionReference = request.TransactionReference,
            Notes = request.Notes
        };
        _store.Payments.Add(payment);

        var invoice = _store.Invoices.FirstOrDefault(i => i.Id == request.InvoiceId);
        if (invoice is not null)
        {
            invoice.Payments.Add(payment);
            if (invoice.IsPaid)
            {
                invoice.Status = InvoiceStatus.Paid;
                invoice.PaidDate = DateTime.UtcNow;
            }
            else if (invoice.AmountPaid > 0)
            {
                invoice.Status = InvoiceStatus.PartiallyPaid;
            }
            invoice.LastModifiedDate = DateTime.UtcNow;
        }

        return Task.FromResult(payment.ToDto(invoice?.InvoiceNumber));
    }

    public Task DeletePaymentAsync(string id)
    {
        var payment = _store.Payments.FirstOrDefault(p => p.Id == id);
        if (payment is not null)
        {
            _store.Payments.Remove(payment);
            var invoice = _store.Invoices.FirstOrDefault(i => i.Id == payment.InvoiceId);
            invoice?.Payments.Remove(payment);
        }
        return Task.CompletedTask;
    }

    private string? InvoiceNumberFor(string? invoiceId)
        => invoiceId is null ? null : _store.Invoices.FirstOrDefault(i => i.Id == invoiceId)?.InvoiceNumber;
}
