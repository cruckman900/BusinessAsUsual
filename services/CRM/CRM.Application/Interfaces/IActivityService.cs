using CRM.Application.DTOs;

namespace CRM.Application.Interfaces;

public interface IActivityService
{
    Task<IEnumerable<ActivityDto>> GetAllActivitiesAsync();
    Task<ActivityDto?> GetActivityByIdAsync(string id);
    Task<IEnumerable<ActivityDto>> GetActivitiesByLeadIdAsync(string leadId);
    Task<IEnumerable<ActivityDto>> GetActivitiesByOpportunityIdAsync(string opportunityId);
    Task<IEnumerable<ActivityDto>> GetActivitiesByCustomerIdAsync(string customerId);
    Task<IEnumerable<ActivityDto>> GetUpcomingActivitiesAsync(int days = 7);
    Task<IEnumerable<ActivityDto>> GetOverdueActivitiesAsync();
    Task<ActivityDto> CreateActivityAsync(CreateActivityRequest request);
    Task<ActivityDto> UpdateActivityAsync(string id, UpdateActivityRequest request);
    Task DeleteActivityAsync(string id);
    Task<ActivityDto> CompleteActivityAsync(string id, string? outcome = null, string? nextSteps = null);
}
