namespace LMS.Domain.DTOs;

public class BadgeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int RequiredPoints { get; set; }
    public bool IsEarned { get; set; }
    public DateTime? EarnedAt { get; set; }
}

public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public int Level { get; set; }
    public int CoursesCompleted { get; set; }
    public int BadgeCount { get; set; }
}

public class GamificationStatsDto
{
    public string EmployeeId { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public int Level { get; set; }
    public int PointsToNextLevel { get; set; }
    public int CoursesCompleted { get; set; }
    public int QuizzesPassed { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public List<BadgeDto> EarnedBadges { get; set; } = new();
    public List<BadgeDto> AvailableBadges { get; set; } = new();
}

public class AwardPointsRequest
{
    public string EmployeeId { get; set; } = string.Empty;
    public int Points { get; set; }
    public string Reason { get; set; } = string.Empty;
}
