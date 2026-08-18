namespace LMS.Domain.Entities;

/// <summary>
/// Represents a learning path (sequence of courses)
/// </summary>
public class LearningPath : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public CourseDifficulty Difficulty { get; set; }
    public int EstimatedHours { get; set; }
    public bool IsPublished { get; set; }
    public string CreatedBy { get; set; } = string.Empty;

    // Navigation
    public List<LearningPathCourse> Courses { get; set; } = new();
    public List<LearningPathEnrollment> Enrollments { get; set; } = new();
}

/// <summary>
/// Represents a course within a learning path with ordering
/// </summary>
public class LearningPathCourse : BaseEntity
{
    public Guid LearningPathId { get; set; }
    public LearningPath LearningPath { get; set; } = null!;

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public int OrderIndex { get; set; }
    public bool IsRequired { get; set; } = true;
}

/// <summary>
/// Represents a learner's enrollment in a learning path
/// </summary>
public class LearningPathEnrollment : BaseEntity
{
    public Guid LearningPathId { get; set; }
    public LearningPath LearningPath { get; set; } = null!;

    public string EmployeeId { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public int ProgressPercentage { get; set; } // 0-100
    public Guid? CurrentCourseId { get; set; }
}

/// <summary>
/// Represents course prerequisites
/// </summary>
public class CoursePrerequisite : BaseEntity
{
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public Guid PrerequisiteCourseId { get; set; }
    public Course PrerequisiteCourse { get; set; } = null!;

    public bool IsRequired { get; set; } = true;
}
