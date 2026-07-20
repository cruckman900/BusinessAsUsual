using Finance.Application.DTOs;
using Finance.Domain.Entities;
using Finance.Domain.Enums;

namespace Finance.Application.Services;

public class MockInvoiceService : IInvoiceService
{
    private readonly FinanceDataStore _store;

    public MockInvoiceService(FinanceDataStore store) => _store = store;

    public Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync()
        => Task.FromResult(_store.Invoices.Select(i => i.ToDto()).AsEnumerable());

    public Task<InvoiceDto?> GetInvoiceByIdAsync(string id)
        => Task.FromResult(_store.Invoices.FirstOrDefault(i => i.Id == id)?.ToDto());

    public Task<IEnumerable<InvoiceDto>> GetInvoicesByCustomerAsync(string customerId)
        => Task.FromResult(_store.Invoices.Where(i => i.CustomerId == customerId).Select(i => i.ToDto()).AsEnumerable());

    public Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceRequest request)
    {
        var invoice = new Invoice
        {
            InvoiceNumber = _store.NextInvoiceNumber(),
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            Currency = request.Currency,
            Status = InvoiceStatus.Draft,
            DueDate = request.DueDate,
            Notes = request.Notes,
            Terms = request.Terms,
            AssignedToEmployeeId = request.AssignedToEmployeeId,
            SourceModule = request.SourceModule,
            SourceReferenceId = request.SourceReferenceId,
            Tags = request.Tags
        };
        invoice.LineItems = request.LineItems.Select(li => li.ToEntity(invoice.Id)).ToList();
        _store.Invoices.Add(invoice);
        return Task.FromResult(invoice.ToDto());
    }

    public Task<InvoiceDto> UpdateInvoiceAsync(string id, UpdateInvoiceRequest request)
    {
        var invoice = _store.Invoices.FirstOrDefault(i => i.Id == id)
            ?? throw new KeyNotFoundException($"Invoice '{id}' not found.");

        invoice.CustomerId = request.CustomerId;
        invoice.CustomerName = request.CustomerName;
        invoice.CustomerEmail = request.CustomerEmail;
        invoice.Status = request.Status;
        invoice.Currency = request.Currency;
        invoice.DueDate = request.DueDate;
        invoice.Notes = request.Notes;
        invoice.Terms = request.Terms;
        invoice.AssignedToEmployeeId = request.AssignedToEmployeeId;
        invoice.Tags = request.Tags;
        invoice.LineItems = request.LineItems.Select(li => li.ToEntity(invoice.Id)).ToList();
        invoice.LastModifiedDate = DateTime.UtcNow;

        return Task.FromResult(invoice.ToDto());
    }

    public Task<InvoiceDto> SendInvoiceAsync(string id)
    {
        var invoice = _store.Invoices.FirstOrDefault(i => i.Id == id)
            ?? throw new KeyNotFoundException($"Invoice '{id}' not found.");
        invoice.Status = InvoiceStatus.Sent;
        invoice.SentDate = DateTime.UtcNow;
        invoice.LastModifiedDate = DateTime.UtcNow;
        return Task.FromResult(invoice.ToDto());
    }

    public Task DeleteInvoiceAsync(string id)
    {
        var invoice = _store.Invoices.FirstOrDefault(i => i.Id == id);
        if (invoice is not null) _store.Invoices.Remove(invoice);
        return Task.CompletedTask;
    }
}
