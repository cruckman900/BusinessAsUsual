namespace LMS.Domain.Entities;

/// <summary>
/// Represents an uploaded media asset (image, video, audio, file)
/// </summary>
public class MediaAsset : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty; // MIME type
    public long FileSizeBytes { get; set; }
    public string StoragePath { get; set; } = string.Empty; // Path in storage system
    public string? ThumbnailPath { get; set; } // For videos/images
    public MediaAssetType AssetType { get; set; }

    // Metadata
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public int? DurationSeconds { get; set; } // For video/audio
    public string? Resolution { get; set; } // For images/videos, e.g., "1920x1080"

    // Ownership & context
    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
}

public enum MediaAssetType
{
    Image,
    Video,
    Audio,
    Document, // PDF, Word, etc.
    Archive, // ZIP, RAR, etc.
    Other
}
