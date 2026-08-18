using LMS.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Services;

public interface IMediaStorageService
{
    Task<(bool Success, string? FilePath, string? ErrorMessage)> SaveFileAsync(
        Stream fileStream, 
        string originalFileName, 
        MediaAssetType assetType,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);
    bool ValidateFileType(string fileName, MediaAssetType assetType);
    bool ValidateFileSize(long fileSizeBytes, MediaAssetType assetType);
}

public class MediaStorageService : IMediaStorageService
{
    private readonly string _uploadBasePath;
    private readonly ILogger<MediaStorageService> _logger;

    // File type restrictions
    private static readonly Dictionary<MediaAssetType, string[]> AllowedExtensions = new()
    {
        { MediaAssetType.Image, new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" } },
        { MediaAssetType.Video, new[] { ".mp4", ".webm", ".mov", ".avi" } },
        { MediaAssetType.Audio, new[] { ".mp3", ".wav", ".ogg", ".m4a" } },
        { MediaAssetType.Document, new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt" } },
        { MediaAssetType.Archive, new[] { ".zip", ".rar", ".7z", ".tar", ".gz" } },
        { MediaAssetType.Other, new[] { "*" } }
    };

    // File size limits (in bytes)
    private static readonly Dictionary<MediaAssetType, long> MaxFileSizes = new()
    {
        { MediaAssetType.Image, 50 * 1024 * 1024 },      // 50 MB
        { MediaAssetType.Video, 200 * 1024 * 1024 },     // 200 MB
        { MediaAssetType.Audio, 50 * 1024 * 1024 },      // 50 MB
        { MediaAssetType.Document, 50 * 1024 * 1024 },   // 50 MB
        { MediaAssetType.Archive, 100 * 1024 * 1024 },   // 100 MB
        { MediaAssetType.Other, 50 * 1024 * 1024 }       // 50 MB
    };

    public MediaStorageService(IWebHostEnvironment environment, ILogger<MediaStorageService> logger)
    {
        _uploadBasePath = Path.Combine(environment.WebRootPath, "uploads", "lms");
        _logger = logger;
        EnsureUploadDirectoriesExist();
    }

    public bool ValidateFileType(string fileName, MediaAssetType assetType)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (!AllowedExtensions.ContainsKey(assetType))
            return false;

        var allowedExts = AllowedExtensions[assetType];

        // "*" means allow all extensions
        if (allowedExts.Contains("*"))
            return true;

        return allowedExts.Contains(extension);
    }

    public bool ValidateFileSize(long fileSizeBytes, MediaAssetType assetType)
    {
        if (!MaxFileSizes.ContainsKey(assetType))
            return false;

        return fileSizeBytes <= MaxFileSizes[assetType];
    }

    public async Task<(bool Success, string? FilePath, string? ErrorMessage)> SaveFileAsync(
        Stream fileStream,
        string originalFileName,
        MediaAssetType assetType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate stream
            if (fileStream == null || !fileStream.CanRead)
            {
                return (false, null, "File stream is null or not readable");
            }

            // Don't check stream.Length as some streams don't support it (like request body streams)
            // The file size validation should be done before calling this method

            // Validate file type
            if (!ValidateFileType(originalFileName, assetType))
            {
                return (false, null, $"File type not allowed for {assetType}");
            }

            // Generate unique filename
            var extension = Path.GetExtension(originalFileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var typeFolderName = assetType.ToString().ToLower();
            var relativeFilePath = Path.Combine("uploads", "lms", typeFolderName, uniqueFileName);
            var fullPath = Path.Combine(_uploadBasePath, typeFolderName, uniqueFileName);

            // Ensure directory exists
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Save file
            using (var fileStreamOutput = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await fileStream.CopyToAsync(fileStreamOutput, cancellationToken);
            }

            _logger.LogInformation("Media file saved: {FilePath}", relativeFilePath);

            // Return relative path (for URLs)
            return (true, $"/{relativeFilePath.Replace("\\", "/")}", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving media file: {FileName}", originalFileName);
            return (false, null, $"Error saving file: {ex.Message}");
        }
    }

    public async Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            // Convert relative path to absolute
            var relativePath = filePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
            var fullPath = Path.Combine(_uploadBasePath, "..", relativePath);

            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath), cancellationToken);
                _logger.LogInformation("Media file deleted: {FilePath}", filePath);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting media file: {FilePath}", filePath);
            return false;
        }
    }

    private void EnsureUploadDirectoriesExist()
    {
        try
        {
            foreach (var assetType in Enum.GetValues<MediaAssetType>())
            {
                var typeFolderName = assetType.ToString().ToLower();
                var folderPath = Path.Combine(_uploadBasePath, typeFolderName);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    _logger.LogInformation("Created upload directory: {FolderPath}", folderPath);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating upload directories");
        }
    }
}
