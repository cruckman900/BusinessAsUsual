namespace LMS.Domain.Entities;

/// <summary>
/// Represents an employee's completion of a course
/// </summary>
public class CourseCompletion : BaseEntity
{
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public string EmployeeId { get; set; } = string.Empty; // Reference to HR employee
    public DateTime StartedAt { get; set; }
    public DateTime CompletedAt { get; set; }

    // Assessment results
    public Guid? FinalAssessmentAttemptId { get; set; }
    public QuizAttempt? FinalAssessmentAttempt { get; set; }
    public decimal FinalScore { get; set; }
    public bool Passed { get; set; }

    // Certificate
    public bool CertificateIssued { get; set; }
    public string? CertificateUrl { get; set; }
    public DateTime? CertificateIssuedDate { get; set; }
    public DateTime? CertificateExpiryDate { get; set; }

    // Progress tracking (JSON)
    public string ProgressData { get; set; } = "{}"; 
    // { "completedModules": ["guid"], "completedLessons": ["guid"], "currentLesson": "guid" }
}
