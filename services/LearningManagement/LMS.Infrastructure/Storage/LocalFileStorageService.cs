using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Storage;

/// <summary>
/// Local file system implementation of blob storage for development/testing
/// In production, this would be swapped with Azure Blob Storage, AWS S3, or MinIO
/// </summary>
public class LocalFileStorageService : IBlobStorageService
{
    private readonly string _storagePath;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(IConfiguration configuration, ILogger<LocalFileStorageService> logger)
    {
        _storagePath = configuration.GetValue<string>("BlobStorage:LocalPath") ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        _logger = logger;

        // Ensure storage directory exists
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
            _logger.LogInformation("Created local blob storage directory: {Path}", _storagePath);
        }
    }

    public async Task<string> UploadFileAsync(string fileName, Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        try
        {
            // Generate unique file name to avoid collisions
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var filePath = Path.Combine(_storagePath, uniqueFileName);

            await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
            await content.CopyToAsync(fileStream, cancellationToken);

            _logger.LogInformation("Uploaded file to local storage: {FileName}", uniqueFileName);

            // Return relative path that can be used to retrieve the file
            return $"/media/{uniqueFileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", fileName);
            throw;
        }
    }

    public async Task<Stream> DownloadFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            // Extract file name from URL (e.g., "/media/filename.jpg" -> "filename.jpg")
            var fileName = Path.GetFileName(fileUrl);
            var filePath = Path.Combine(_storagePath, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {fileName}");
            }

            var memoryStream = new MemoryStream();
            await using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
            {
                await fileStream.CopyToAsync(memoryStream, cancellationToken);
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file: {FileUrl}", fileUrl);
            throw;
        }
    }

    public Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var fileName = Path.GetFileName(fileUrl);
            var filePath = Path.Combine(_storagePath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("Deleted file from local storage: {FileName}", fileName);
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FileUrl}", fileUrl);
            throw;
        }
    }

    public Task<bool> FileExistsAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var fileName = Path.GetFileName(fileUrl);
            var filePath = Path.Combine(_storagePath, fileName);
            return Task.FromResult(File.Exists(filePath));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking file existence: {FileUrl}", fileUrl);
            return Task.FromResult(false);
        }
    }
}
