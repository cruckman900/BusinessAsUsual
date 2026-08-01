using Finance.Application.DTOs;
using Finance.Domain.Enums;

namespace Finance.Application.Services;

public class MockVendorPaymentService : IVendorPaymentService
{
    private readonly List<VendorPaymentDto> _payments = new();

    public MockVendorPaymentService()
    {
        // Seed with sample data
        _payments.Add(new VendorPaymentDto
        {
            Id = Guid.NewGuid().ToString(),
            VendorName = "Office Supplies Co",
            Amount = 3000m,
            Currency = Currency.USD.ToString(),
            Method = PaymentMethod.BankTransfer.ToString(),
            Status = PaymentStatus.Completed.ToString(),
            PaymentDate = DateTime.Now.AddDays(-10),
            TransactionReference = "TXN-987654",
            CreatedDate = DateTime.Now.AddDays(-10)
        });
    }

    public Task<IEnumerable<VendorPaymentDto>> GetAllVendorPaymentsAsync()
        => Task.FromResult<IEnumerable<VendorPaymentDto>>(_payments);

    public Task<VendorPaymentDto?> GetVendorPaymentByIdAsync(string id)
        => Task.FromResult(_payments.FirstOrDefault(p => p.Id == id));

    public Task<IEnumerable<VendorPaymentDto>> GetVendorPaymentsByBillAsync(string billId)
        => Task.FromResult<IEnumerable<VendorPaymentDto>>(_payments.Where(p => p.BillId == billId));

    public Task<VendorPaymentDto> RecordVendorPaymentAsync(RecordVendorPaymentRequest request)
    {
        var payment = new VendorPaymentDto
        {
            Id = Guid.NewGuid().ToString(),
            BillId = request.BillId,
            VendorId = request.VendorId,
            Amount = request.Amount,
            Currency = request.Currency.ToString(),
            Method = request.Method.ToString(),
            Status = request.Status.ToString(),
            PaymentDate = request.PaymentDate ?? DateTime.Now,
            TransactionReference = request.TransactionReference,
            Notes = request.Notes,
            CreatedDate = DateTime.Now
        };

        _payments.Add(payment);
        return Task.FromResult(payment);
    }

    public Task DeleteVendorPaymentAsync(string id)
    {
        var payment = _payments.FirstOrDefault(p => p.Id == id);
        if (payment is not null)
            _payments.Remove(payment);

        return Task.CompletedTask;
    }
}
