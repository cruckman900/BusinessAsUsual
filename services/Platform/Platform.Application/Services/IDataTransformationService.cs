using Platform.Application.DTOs.Import;

namespace Platform.Application.Services;

/// <summary>
/// Service for applying data transformations during import
/// </summary>
public interface IDataTransformationService
{
    /// <summary>
    /// Applies a transformation to a single value
    /// </summary>
    TransformationResult ApplyTransformation(string value, DataTransformation transformation);

    /// <summary>
    /// Applies a transformation to a row of data
    /// </summary>
    TransformationResult ApplyTransformationToRow(Dictionary<string, string> row, string sourceColumn, DataTransformation transformation);

    /// <summary>
    /// Gets a list of available transformations
    /// </summary>
    List<string> GetAvailableTransformations();

    /// <summary>
    /// Gets the parameters required for a specific transformation type
    /// </summary>
    List<string> GetTransformationParameters(string transformationType);

    /// <summary>
    /// Validates that a transformation is configured correctly
    /// </summary>
    bool ValidateTransformation(DataTransformation transformation, out string? errorMessage);
}
