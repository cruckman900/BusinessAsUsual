using LMS.Application.Features.Media.Commands;
using LMS.Domain.Entities;
using LMS.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Web.Controllers;

[ApiController]
[Route("api/lms/media")]
public class LMSMediaController : ControllerBase
{
    private readonly IMediaStorageService _mediaStorageService;
    private readonly UploadMediaCommandHandler _uploadHandler;
    private readonly ILogger<LMSMediaController> _logger;

    public LMSMediaController(
        IMediaStorageService mediaStorageService,
        UploadMediaCommandHandler uploadHandler,
        ILogger<LMSMediaController> logger)
    {
        _mediaStorageService = mediaStorageService;
        _uploadHandler = uploadHandler;
        _logger = logger;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(536_870_912)] // 512 MB
    [RequestFormLimits(MultipartBodyLengthLimit = 536_870_912)]
    public async Task<IActionResult> UploadMedia(
        [FromForm] IFormFile file,
        [FromForm] string assetType,
        [FromForm] Guid? courseId = null,
        [FromForm] string? altText = null,
        [FromForm] string? caption = null)
    {
        try
        {
            _logger.LogInformation("Upload request received - File: {HasFile}, AssetType: {AssetType}", 
                file != null, assetType);

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Upload failed: No file or file length is 0. File null: {IsNull}, Length: {Length}", 
                    file == null, file?.Length ?? 0);
                return BadRequest(new { error = "No file uploaded" });
            }

            _logger.LogInformation("File details: Name={FileName}, Size={Size}, ContentType={ContentType}", 
                file.FileName, file.Length, file.ContentType);

            // Parse asset type
            if (!Enum.TryParse<MediaAssetType>(assetType, true, out var mediaAssetType))
                return BadRequest(new { error = "Invalid asset type" });

            // Validate file type
            if (!_mediaStorageService.ValidateFileType(file.FileName, mediaAssetType))
                return BadRequest(new { error = $"File type not allowed for {mediaAssetType}" });

            // Validate file size
            if (!_mediaStorageService.ValidateFileSize(file.Length, mediaAssetType))
                return BadRequest(new { error = $"File size exceeds maximum for {mediaAssetType}" });

            // Save file to storage
            using var stream = file.OpenReadStream();

            _logger.LogInformation("Receiving file upload: {FileName}, Size: {Size} bytes, ContentType: {ContentType}", 
                file.FileName, file.Length, file.ContentType);

            var (success, filePath, errorMessage) = await _mediaStorageService.SaveFileAsync(
                stream,
                file.FileName,
                mediaAssetType);

            if (!success || string.IsNullOrEmpty(filePath))
                return BadRequest(new { error = errorMessage ?? "Failed to save file" });

            // Create media asset record
            var command = new UploadMediaCommand
            {
                FileStream = Stream.Null, // File already saved
                OriginalFileName = file.FileName,
                StoragePath = filePath,
                ContentType = file.ContentType,
                FileSizeBytes = file.Length,
                AssetType = mediaAssetType,
                CourseId = courseId,
                AltText = altText,
                Caption = caption,
                UploadedBy = "admin" // TODO: Get from auth
            };

            var result = await _uploadHandler.HandleAsync(command);

            if (!result.Success)
                return BadRequest(new { error = result.ErrorMessage });

            return Ok(new
            {
                id = result.Data,
                url = filePath,
                filename = file.FileName,
                contentType = file.ContentType,
                size = file.Length,
                assetType = mediaAssetType.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading media file");
            return StatusCode(500, new { error = "Internal server error uploading file" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMediaAsset(Guid id)
    {
        // TODO: Implement get media asset details
        return NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMediaAsset(Guid id)
    {
        // TODO: Implement delete media asset
        return NoContent();
    }
}
