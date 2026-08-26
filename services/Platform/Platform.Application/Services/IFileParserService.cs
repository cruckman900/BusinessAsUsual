using Platform.Application.DTOs.Import;

namespace Platform.Application.Services;

/// <summary>
/// Service for parsing various file formats into structured data
/// </summary>
public interface IFileParserService
{
    /// <summary>
    /// Parses a file from a stream
    /// </summary>
    Task<ParsedData> ParseFileAsync(Stream fileStream, string fileName, ImportFileType fileType);

    /// <summary>
    /// Parses text content (CSV, tab-delimited, etc.)
    /// </summary>
    Task<ParsedData> ParseTextAsync(string content, ImportFileType fileType);

    /// <summary>
    /// Detects file type from file name
    /// </summary>
    ImportFileType DetectFileType(string fileName);

    /// <summary>
    /// Validates that a file can be parsed
    /// </summary>
    Task<bool> ValidateFileAsync(Stream fileStream, string fileName);
}
