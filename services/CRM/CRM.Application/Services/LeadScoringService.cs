using CRM.Application.DTOs;
using CRM.Application.Services;

namespace CRM.Application.Services;

public class LeadScoringService : ILeadScoringService
{
    private readonly ILeadService _leadService;
    private LeadScoringConfigDto _config;

    public LeadScoringService(ILeadService leadService)
    {
        _leadService = leadService;
        _config = GetDefaultConfig();
    }

    public async Task<LeadScoreDto> CalculateLeadScoreAsync(string leadId)
    {
        var lead = await _leadService.GetLeadByIdAsync(leadId);
        if (lead == null)
            throw new KeyNotFoundException($"Lead {leadId} not found");

        return CalculateScore(lead);
    }

    public async Task<IEnumerable<LeadScoreDto>> CalculateAllLeadScoresAsync()
    {
        var leads = await _leadService.GetAllLeadsAsync();
        return leads.Select(CalculateScore).OrderByDescending(s => s.TotalScore);
    }

    public Task<LeadScoringConfigDto> GetScoringConfigAsync()
        => Task.FromResult(_config);

    public Task<LeadScoringConfigDto> UpdateScoringConfigAsync(LeadScoringConfigDto config)
    {
        _config = config;
        return Task.FromResult(_config);
    }

    public async Task<IEnumerable<LeadDto>> GetHotLeadsAsync()
    {
        var leads = await _leadService.GetAllLeadsAsync();
        var scoredLeads = leads.Select(l => new { Lead = l, Score = CalculateScore(l) });
        return scoredLeads
            .Where(sl => sl.Score.ScoreLevel == "Hot")
            .OrderByDescending(sl => sl.Score.TotalScore)
            .Select(sl => sl.Lead);
    }

    private LeadScoreDto CalculateScore(LeadDto lead)
    {
        var scoreDto = new LeadScoreDto
        {
            LeadId = lead.Id,
            LastCalculated = DateTime.Now
        };

        int totalScore = 0;
        var reasons = new List<string>();

        // Demographic Scoring
        if (!string.IsNullOrWhiteSpace(lead.Company))
        {
            totalScore += 10;
            scoreDto.ScoreBreakdown["Company Provided"] = 10;
            reasons.Add("Company information provided (+10)");
        }

        if (!string.IsNullOrWhiteSpace(lead.JobTitle))
        {
            var title = lead.JobTitle.ToLower();
            if (title.Contains("ceo") || title.Contains("president") || title.Contains("owner"))
            {
                totalScore += 20;
                scoreDto.ScoreBreakdown["Executive Title"] = 20;
                reasons.Add("Executive-level title (+20)");
            }
            else if (title.Contains("director") || title.Contains("manager") || title.Contains("vp"))
            {
                totalScore += 15;
                scoreDto.ScoreBreakdown["Manager Title"] = 15;
                reasons.Add("Manager/Director title (+15)");
            }
            else
            {
                totalScore += 5;
                scoreDto.ScoreBreakdown["Title Provided"] = 5;
                reasons.Add("Job title provided (+5)");
            }
        }

        // Engagement Scoring
        if (lead.ActivityCount > 0)
        {
            int activityPoints = Math.Min(lead.ActivityCount * 5, 25);
            totalScore += activityPoints;
            scoreDto.ScoreBreakdown["Activity Engagement"] = activityPoints;
            reasons.Add($"{lead.ActivityCount} activities logged (+{activityPoints})");
        }

        if (lead.LastContactedDate.HasValue)
        {
            var daysSinceContact = (DateTime.Now - lead.LastContactedDate.Value).Days;
            if (daysSinceContact < 7)
            {
                totalScore += 15;
                scoreDto.ScoreBreakdown["Recent Contact"] = 15;
                reasons.Add("Recent contact within 7 days (+15)");
            }
            else if (daysSinceContact < 30)
            {
                totalScore += 10;
                scoreDto.ScoreBreakdown["Recent Contact"] = 10;
                reasons.Add("Contact within 30 days (+10)");
            }
        }

        // Value Scoring
        if (lead.EstimatedValue.HasValue)
        {
            if (lead.EstimatedValue.Value >= 100000)
            {
                totalScore += 30;
                scoreDto.ScoreBreakdown["High Value"] = 30;
                reasons.Add($"High estimated value ${lead.EstimatedValue:N0} (+30)");
            }
            else if (lead.EstimatedValue.Value >= 50000)
            {
                totalScore += 20;
                scoreDto.ScoreBreakdown["Medium Value"] = 20;
                reasons.Add($"Medium estimated value ${lead.EstimatedValue:N0} (+20)");
            }
            else if (lead.EstimatedValue.Value >= 10000)
            {
                totalScore += 10;
                scoreDto.ScoreBreakdown["Estimated Value"] = 10;
                reasons.Add($"Estimated value ${lead.EstimatedValue:N0} (+10)");
            }
        }

        // Source Scoring (higher quality sources score better)
        if (!string.IsNullOrWhiteSpace(lead.Source))
        {
            switch (lead.Source.ToLower())
            {
                case "referral":
                    totalScore += 25;
                    scoreDto.ScoreBreakdown["Referral Source"] = 25;
                    reasons.Add("Referral source (+25)");
                    break;
                case "website":
                    totalScore += 15;
                    scoreDto.ScoreBreakdown["Website Source"] = 15;
                    reasons.Add("Inbound website lead (+15)");
                    break;
                case "event":
                case "webinar":
                    totalScore += 20;
                    scoreDto.ScoreBreakdown["Event Source"] = 20;
                    reasons.Add("Event/webinar attendee (+20)");
                    break;
                case "cold call":
                    totalScore += 5;
                    scoreDto.ScoreBreakdown["Cold Source"] = 5;
                    reasons.Add("Cold outreach (+5)");
                    break;
                default:
                    totalScore += 10;
                    scoreDto.ScoreBreakdown["Other Source"] = 10;
                    reasons.Add($"Source: {lead.Source} (+10)");
                    break;
            }
        }

        // Status Bonus
        if (lead.Status.ToLower() == "qualified")
        {
            totalScore += 20;
            scoreDto.ScoreBreakdown["Qualified Status"] = 20;
            reasons.Add("Lead is qualified (+20)");
        }

        // Completeness Bonus
        int completeness = 0;
        if (!string.IsNullOrWhiteSpace(lead.Phone)) completeness++;
        if (!string.IsNullOrWhiteSpace(lead.Company)) completeness++;
        if (!string.IsNullOrWhiteSpace(lead.JobTitle)) completeness++;
        if (!string.IsNullOrWhiteSpace(lead.LinkedInProfile)) completeness++;
        if (!string.IsNullOrWhiteSpace(lead.AddressLine1)) completeness++;

        if (completeness >= 4)
        {
            totalScore += 10;
            scoreDto.ScoreBreakdown["Complete Profile"] = 10;
            reasons.Add("Complete profile (+10)");
        }

        // Time Decay
        var daysSinceCreated = (DateTime.Now - lead.CreatedDate).Days;
        if (daysSinceCreated > _config.ScoreDecayDays)
        {
            var monthsOld = daysSinceCreated / 30;
            var decay = Math.Min(monthsOld * _config.DecayPercentPerMonth, 50);
            int decayPoints = (int)(totalScore * (decay / 100.0));
            totalScore -= decayPoints;
            scoreDto.ScoreBreakdown["Time Decay"] = -decayPoints;
            reasons.Add($"Age decay: {monthsOld} months old (-{decayPoints})");
        }

        scoreDto.TotalScore = Math.Max(totalScore, 0);
        scoreDto.ScoringReasons = reasons;

        // Determine level
        if (scoreDto.TotalScore >= _config.HotThreshold)
            scoreDto.ScoreLevel = "Hot";
        else if (scoreDto.TotalScore >= _config.WarmThreshold)
            scoreDto.ScoreLevel = "Warm";
        else
            scoreDto.ScoreLevel = "Cold";

        return scoreDto;
    }

    private static LeadScoringConfigDto GetDefaultConfig()
    {
        return new LeadScoringConfigDto
        {
            HotThreshold = 70,
            WarmThreshold = 40,
            ScoreDecayDays = 30,
            DecayPercentPerMonth = 10,
            Rules = new List<ScoringRule>
            {
                new() { RuleName = "Company Provided", Category = "Demographics", Points = 10, IsActive = true },
                new() { RuleName = "Executive Title", Category = "Demographics", Points = 20, IsActive = true },
                new() { RuleName = "Manager Title", Category = "Demographics", Points = 15, IsActive = true },
                new() { RuleName = "Activity Per Interaction", Category = "Engagement", Points = 5, IsActive = true },
                new() { RuleName = "Recent Contact (7 days)", Category = "Engagement", Points = 15, IsActive = true },
                new() { RuleName = "High Value (100k+)", Category = "Value", Points = 30, IsActive = true },
                new() { RuleName = "Medium Value (50k+)", Category = "Value", Points = 20, IsActive = true },
                new() { RuleName = "Referral Source", Category = "Source", Points = 25, IsActive = true },
                new() { RuleName = "Event Source", Category = "Source", Points = 20, IsActive = true },
                new() { RuleName = "Qualified Status", Category = "Status", Points = 20, IsActive = true },
                new() { RuleName = "Complete Profile", Category = "Completeness", Points = 10, IsActive = true }
            }
        };
    }
}
