using CRM.Application.DTOs;

namespace CRM.Application.Services;

public interface ILeadScoringService
{
    Task<LeadScoreDto> CalculateLeadScoreAsync(string leadId);
    Task<IEnumerable<LeadScoreDto>> CalculateAllLeadScoresAsync();
    Task<LeadScoringConfigDto> GetScoringConfigAsync();
    Task<LeadScoringConfigDto> UpdateScoringConfigAsync(LeadScoringConfigDto config);
    Task<IEnumerable<LeadDto>> GetHotLeadsAsync();
}
