using Finance.Application.DTOs;

namespace Finance.Application.Services;

public interface ICollectionService
{
    Task<AgingReportDto> GenerateAgingReportAsync(DateTime? asOfDate = null);
    Task<IEnumerable<CollectionActivityDto>> GetAllActivitiesAsync();
    Task<IEnumerable<CollectionActivityDto>> GetActivitiesByInvoiceAsync(string invoiceId);
    Task<IEnumerable<CollectionActivityDto>> GetActivitiesByCustomerAsync(string customerId);
    Task<CollectionActivityDto?> GetActivityByIdAsync(string id);
    Task<CollectionActivityDto> CreateActivityAsync(CreateCollectionActivityRequest request);
    Task<CollectionActivityDto> UpdateActivityAsync(string id, UpdateCollectionActivityRequest request);
    Task DeleteActivityAsync(string id);
}
