namespace CRM.Application.DTOs;

public class LeadScoreDto
{
    public string LeadId { get; set; } = string.Empty;
    public int TotalScore { get; set; }
    public string ScoreLevel { get; set; } = string.Empty; // Hot, Warm, Cold
    public Dictionary<string, int> ScoreBreakdown { get; set; } = new();
    public DateTime LastCalculated { get; set; }
    public List<string> ScoringReasons { get; set; } = new();
}

public class ScoringRule
{
    public string RuleName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Points { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

public class LeadScoringConfigDto
{
    public List<ScoringRule> Rules { get; set; } = new();
    public int HotThreshold { get; set; } = 70;
    public int WarmThreshold { get; set; } = 40;
    public int ScoreDecayDays { get; set; } = 30;
    public int DecayPercentPerMonth { get; set; } = 10;
}
