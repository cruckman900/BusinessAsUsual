namespace LMS.Domain.Entities;

/// <summary>
/// Tracks detailed progress for a learner through a course
/// </summary>
public class DetailedLearnerProgress : BaseEntity
{
    /// <summary>
    /// The user/employee ID tracking progress
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The course being tracked
    /// </summary>
    public Guid CourseId { get; set; }
    public Course? Course { get; set; }

    /// <summary>
    /// Overall completion percentage (0-100)
    /// </summary>
    public decimal PercentComplete { get; set; } = 0;

    /// <summary>
    /// When the learner first started the course
    /// </summary>
    public DateTime? StartedDate { get; set; }

    /// <summary>
    /// When the learner last accessed the course
    /// </summary>
    public DateTime? LastAccessedDate { get; set; }

    /// <summary>
    /// When the course was completed (100%)
    /// </summary>
    public DateTime? CompletionDate { get; set; }

    /// <summary>
    /// Final score/grade if applicable (0-100)
    /// </summary>
    public decimal? Score { get; set; }

    /// <summary>
    /// Time spent in minutes
    /// </summary>
    public int TimeSpentMinutes { get; set; } = 0;

    /// <summary>
    /// Current module/section the learner is on
    /// </summary>
    public string? CurrentModule { get; set; }

    /// <summary>
    /// Number of attempts (for courses with assessments)
    /// </summary>
    public int Attempts { get; set; } = 0;

    /// <summary>
    /// Whether the course is currently in progress
    /// </summary>
    public bool IsInProgress => StartedDate.HasValue && !CompletionDate.HasValue;

    /// <summary>
    /// Whether the course has been completed
    /// </summary>
    public bool IsCompleted => CompletionDate.HasValue;
}
