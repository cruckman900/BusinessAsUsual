namespace LMS.Contracts.DTOs;

public class LessonDto
{
    public Guid Id { get; set; }
    public Guid ModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public List<ContentBlockDto> ContentBlocks { get; set; } = new();
}
