namespace LMS.Contracts.DTOs;

public class ModuleDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public List<LessonDto> Lessons { get; set; } = new();
}
