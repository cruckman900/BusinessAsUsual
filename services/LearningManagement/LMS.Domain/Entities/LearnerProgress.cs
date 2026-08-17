namespace LMS.Domain.Entities;

/// <summary>
/// Tracks learner progress through a course
/// </summary>
public class LearnerProgress : BaseEntity
{
    public string EmployeeId { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public Guid? CurrentModuleId { get; set; }
    public Guid? CurrentLessonId { get; set; }

    public List<Guid> CompletedModules { get; set; } = new();
    public List<Guid> CompletedLessons { get; set; } = new();
    public List<Guid> CompletedQuizzes { get; set; } = new();

    public int ProgressPercentage { get; set; } // 0-100
    public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;
}
