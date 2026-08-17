namespace LMS.Domain.Entities;

/// <summary>
/// Represents a content block within a lesson (text, image, video, quiz, etc.)
/// Uses hybrid storage: metadata in columns, content as JSON
/// </summary>
public class ContentBlock : BaseEntity
{
    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;

    public ContentBlockType BlockType { get; set; }
    public int OrderIndex { get; set; }

    /// <summary>
    /// JSON content specific to the block type
    /// - Text: { "content": "HTML or markdown", "textAlign": "left" }
    /// - Image: { "url": "/media/...", "alt": "...", "caption": "..." }
    /// - Video: { "url": "/media/...", "thumbnail": "...", "duration": 120 }
    /// - Audio: { "url": "/media/...", "duration": 60 }
    /// - Quiz: { "quizId": "guid" }
    /// - Divider: { "style": "solid|dashed" }
    /// </summary>
    public string JsonContent { get; set; } = "{}";

    // Optional reference to quiz (if BlockType is Quiz)
    public Guid? QuizId { get; set; }
    public Quiz? Quiz { get; set; }
}

public enum ContentBlockType
{
    Text,
    Heading,
    Image,
    Video,
    Audio,
    Quiz,
    Divider,
    CodeSnippet,
    Callout // Info boxes, warnings, tips
}
