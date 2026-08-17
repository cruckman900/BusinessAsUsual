namespace LMS.Domain.Entities;

/// <summary>
/// Represents a lesson within a module (contains content blocks)
/// </summary>
public class Lesson : BaseEntity
{
    public Guid ModuleId { get; set; }
    public Module Module { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public int EstimatedDurationMinutes { get; set; }

    // Navigation properties
    public ICollection<ContentBlock> ContentBlocks { get; set; } = new List<ContentBlock>();
    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}
