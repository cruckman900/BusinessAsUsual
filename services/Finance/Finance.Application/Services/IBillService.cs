using Finance.Application.DTOs;

namespace Finance.Application.Services;

public interface IBillService
{
    Task<IEnumerable<BillDto>> GetAllBillsAsync();
    Task<BillDto?> GetBillByIdAsync(string id);
    Task<IEnumerable<BillDto>> GetBillsByVendorAsync(string vendorId);
    Task<BillDto> CreateBillAsync(CreateBillRequest request);
    Task<BillDto> UpdateBillAsync(string id, UpdateBillRequest request);
    Task DeleteBillAsync(string id);
}
