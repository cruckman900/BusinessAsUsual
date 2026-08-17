namespace LMS.Domain.Entities;

/// <summary>
/// Represents a notification sent to a learner
/// </summary>
public class Notification : BaseEntity
{
    public string EmployeeId { get; set; } = string.Empty;

    public NotificationType Type { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string? ActionUrl { get; set; }

    public string? ActionText { get; set; }

    public bool IsRead { get; set; } = false;

    public DateTime? ReadAt { get; set; }

    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    // Related entity references
    public Guid? CourseId { get; set; }
    public Guid? AssignmentId { get; set; }
    public Guid? CertificateId { get; set; }
    public Guid? QuizAttemptId { get; set; }

    // Email tracking
    public bool EmailSent { get; set; } = false;
    public DateTime? EmailSentAt { get; set; }
    public string? EmailError { get; set; }
}

public enum NotificationType
{
    CourseAssigned,
    CourseStarted,
    CourseCompleted,
    CertificateIssued,
    QuizPassed,
    QuizFailed,
    AssignmentDueSoon,
    AssignmentOverdue,
    CertificateExpiring,
    SystemAnnouncement
}

public enum NotificationPriority
{
    Low,
    Normal,
    High,
    Urgent
}
