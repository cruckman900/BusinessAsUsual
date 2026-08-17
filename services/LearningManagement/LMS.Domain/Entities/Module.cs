namespace LMS.Domain.Entities;

/// <summary>
/// Represents a module within a course (high-level grouping of lessons)
/// </summary>
public class Module : BaseEntity
{
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; } // For sorting/ordering

    // Navigation properties
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
