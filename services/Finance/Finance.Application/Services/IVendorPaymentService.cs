using Finance.Application.DTOs;

namespace Finance.Application.Services;

public interface IVendorPaymentService
{
    Task<IEnumerable<VendorPaymentDto>> GetAllVendorPaymentsAsync();
    Task<VendorPaymentDto?> GetVendorPaymentByIdAsync(string id);
    Task<IEnumerable<VendorPaymentDto>> GetVendorPaymentsByBillAsync(string billId);
    Task<VendorPaymentDto> RecordVendorPaymentAsync(RecordVendorPaymentRequest request);
    Task DeleteVendorPaymentAsync(string id);
}
