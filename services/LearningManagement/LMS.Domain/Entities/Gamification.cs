namespace LMS.Domain.Entities;

/// <summary>
/// Represents a badge that can be earned
/// </summary>
public class Badge : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public BadgeType Type { get; set; }
    public int RequiredPoints { get; set; }
    public string? RequiredCourseId { get; set; }
    public int? RequiredCompletionCount { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Represents a badge earned by a learner
/// </summary>
public class EarnedBadge : BaseEntity
{
    public Guid BadgeId { get; set; }
    public Badge Badge { get; set; } = null!;

    public string EmployeeId { get; set; } = string.Empty;
    public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
    public string? Reason { get; set; }
}

/// <summary>
/// Represents an achievement (milestone)
/// </summary>
public class Achievement : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public AchievementType Type { get; set; }
    public int TargetValue { get; set; }
    public int PointsAwarded { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Represents learner points and gamification stats
/// </summary>
public class LearnerGamification : BaseEntity
{
    public string EmployeeId { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public int Level { get; set; } = 1;
    public int CoursesCompleted { get; set; }
    public int QuizzesPassed { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateTime? LastActivityDate { get; set; }

    public List<EarnedBadge> Badges { get; set; } = new();
}

public enum BadgeType
{
    Completion,
    Mastery,
    Streak,
    Social,
    Special
}

public enum AchievementType
{
    CourseCompletion,
    QuizPerfection,
    LearningStreak,
    PointsMilestone,
    TimeInvestment
}
