namespace LMS.Infrastructure.Storage;

/// <summary>
/// Abstraction for blob storage (videos, images, audio, documents)
/// </summary>
public interface IBlobStorageService
{
    /// <summary>
    /// Upload a file to blob storage
    /// </summary>
    /// <param name="fileName">Name of the file</param>
    /// <param name="content">File content stream</param>
    /// <param name="contentType">MIME type of the file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>URL or path to access the uploaded file</returns>
    Task<string> UploadFileAsync(string fileName, Stream content, string contentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Download a file from blob storage
    /// </summary>
    /// <param name="fileUrl">URL or path to the file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File content stream</returns>
    Task<Stream> DownloadFileAsync(string fileUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a file from blob storage
    /// </summary>
    /// <param name="fileUrl">URL or path to the file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a file exists in blob storage
    /// </summary>
    /// <param name="fileUrl">URL or path to the file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<bool> FileExistsAsync(string fileUrl, CancellationToken cancellationToken = default);
}
