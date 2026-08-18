using LMS.Application.Common;
using LMS.Application.Services;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Media.Commands;

public class UploadMediaCommandHandler : ICommandHandler<UploadMediaCommand, Result<Guid>>
{
    private readonly IMediaAssetRepository _mediaAssetRepository;
    private readonly ILogger<UploadMediaCommandHandler> _logger;

    public UploadMediaCommandHandler(
        IMediaAssetRepository mediaAssetRepository,
        ILogger<UploadMediaCommandHandler> logger)
    {
        _mediaAssetRepository = mediaAssetRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> HandleAsync(UploadMediaCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate - file stream check removed because file is saved before handler is called
            if (string.IsNullOrWhiteSpace(command.OriginalFileName))
                return Result<Guid>.Fail("File name is required");

            if (string.IsNullOrWhiteSpace(command.StoragePath))
                return Result<Guid>.Fail("Storage path is required");

            if (command.FileSizeBytes <= 0)
                return Result<Guid>.Fail("File size must be greater than zero");

            // Create MediaAsset entity
            var mediaAsset = new MediaAsset
            {
                Id = Guid.NewGuid(),
                FileName = Path.GetFileName(command.StoragePath),
                OriginalFileName = command.OriginalFileName,
                ContentType = command.ContentType,
                FileSizeBytes = command.FileSizeBytes,
                StoragePath = command.StoragePath,
                AssetType = command.AssetType,
                CourseId = command.CourseId,
                AltText = command.AltText,
                Caption = command.Caption,
                UploadedBy = command.UploadedBy,
                UploadedDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = command.UploadedBy
            };

            await _mediaAssetRepository.AddAsync(mediaAsset, cancellationToken);

            _logger.LogInformation(
                "Media uploaded: {AssetId} - {FileName} ({AssetType})",
                mediaAsset.Id,
                mediaAsset.OriginalFileName,
                mediaAsset.AssetType);

            return Result<Guid>.Ok(mediaAsset.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading media: {FileName}", command.OriginalFileName);
            return Result<Guid>.Fail($"Error uploading media: {ex.Message}");
        }
    }
}
