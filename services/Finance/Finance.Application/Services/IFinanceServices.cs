using Finance.Application.DTOs;

namespace Finance.Application.Services;

public interface IInvoiceService
{
    Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync();
    Task<InvoiceDto?> GetInvoiceByIdAsync(string id);
    Task<IEnumerable<InvoiceDto>> GetInvoicesByCustomerAsync(string customerId);
    Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceRequest request);
    Task<InvoiceDto> UpdateInvoiceAsync(string id, UpdateInvoiceRequest request);
    Task<InvoiceDto> SendInvoiceAsync(string id);
    Task DeleteInvoiceAsync(string id);
}

public interface IPaymentService
{
    Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync();
    Task<PaymentDto?> GetPaymentByIdAsync(string id);
    Task<IEnumerable<PaymentDto>> GetPaymentsByInvoiceAsync(string invoiceId);
    Task<PaymentDto> RecordPaymentAsync(RecordPaymentRequest request);
    Task DeletePaymentAsync(string id);
}

public interface IFinanceReportService
{
    Task<FinanceSummaryDto> GetSummaryAsync();
}
