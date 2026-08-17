namespace LMS.Contracts.DTOs;

public class ContentBlockDto
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public string BlockType { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public object? Content { get; set; } // Deserialized JSON content
    public Guid? QuizId { get; set; }
}
