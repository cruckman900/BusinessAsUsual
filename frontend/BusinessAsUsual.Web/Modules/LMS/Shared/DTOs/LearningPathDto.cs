namespace BusinessAsUsual.Web.Modules.LMS.Shared.DTOs;

public class LearningPathDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public int EstimatedHours { get; set; }
    public bool IsPublished { get; set; }
    public int CourseCount { get; set; }
    public List<LearningPathCourseDto> Courses { get; set; } = new();
}

public class LearningPathCourseDto
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public bool IsRequired { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsLocked { get; set; }
}

public class LearningPathProgressDto
{
    public Guid PathId { get; set; }
    public string PathTitle { get; set; } = string.Empty;
    public int ProgressPercentage { get; set; }
    public int CompletedCourses { get; set; }
    public int TotalCourses { get; set; }
    public Guid? CurrentCourseId { get; set; }
    public string? CurrentCourseTitle { get; set; }
    public DateTime EnrolledAt { get; set; }
    public DateTime? EstimatedCompletionDate { get; set; }
}

public class CreateLearningPathRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Difficulty { get; set; } = "Beginner";
    public int EstimatedHours { get; set; }
    public List<PathCourseInput> Courses { get; set; } = new();
}

public class PathCourseInput
{
    public Guid CourseId { get; set; }
    public int OrderIndex { get; set; }
    public bool IsRequired { get; set; } = true;
}
