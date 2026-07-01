using CRM.Application.DTOs;

namespace CRM.Application.Services;

public interface ILeadService
{
    Task<IEnumerable<LeadDto>> GetAllLeadsAsync();
    Task<LeadDto?> GetLeadByIdAsync(string id);
    Task<LeadDto> CreateLeadAsync(CreateLeadRequest request);
    Task<LeadDto> UpdateLeadAsync(string id, UpdateLeadRequest request);
    Task DeleteLeadAsync(string id);
    Task<LeadDto> ConvertLeadToCustomerAsync(string id);
}

public interface IOpportunityService
{
    Task<IEnumerable<OpportunityDto>> GetAllOpportunitiesAsync();
    Task<OpportunityDto?> GetOpportunityByIdAsync(string id);
    Task<IEnumerable<OpportunityDto>> GetOpportunitiesByCustomerAsync(string customerId);
    Task<OpportunityDto> CreateOpportunityAsync(CreateOpportunityRequest request);
    Task<OpportunityDto> UpdateOpportunityAsync(string id, UpdateOpportunityRequest request);
    Task DeleteOpportunityAsync(string id);
}

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
    Task<CustomerDto?> GetCustomerByIdAsync(string id);
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerRequest request);
    Task<CustomerDto> UpdateCustomerAsync(string id, UpdateCustomerRequest request);
    Task DeleteCustomerAsync(string id);
}
