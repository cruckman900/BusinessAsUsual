using LMS.Application.Common;
using LMS.Domain.Entities;

namespace LMS.Application.Features.Media.Commands;

public class UploadMediaCommand : ICommand<Result<Guid>>
{
    public Stream FileStream { get; set; } = null!;
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public MediaAssetType AssetType { get; set; }
    public Guid? CourseId { get; set; }
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
}
