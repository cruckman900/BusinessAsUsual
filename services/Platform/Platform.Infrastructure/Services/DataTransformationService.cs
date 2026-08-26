using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Platform.Application.DTOs.Import;
using Platform.Application.Services;

namespace Platform.Infrastructure.Services;

/// <summary>
/// Implementation of data transformation service
/// </summary>
public class DataTransformationService : IDataTransformationService
{
    private readonly ILogger<DataTransformationService> _logger;

    public DataTransformationService(ILogger<DataTransformationService> logger)
    {
        _logger = logger;
    }

    public List<string> GetAvailableTransformations()
    {
        return new List<string>
        {
            TransformationType.SplitFullName,
            TransformationType.TrimWhitespace,
            TransformationType.FormatDate,
            TransformationType.UpperCase,
            TransformationType.LowerCase,
            TransformationType.TitleCase,
            TransformationType.RemoveSpecialChars,
            TransformationType.PadLeft,
            TransformationType.PadRight,
            TransformationType.Substring,
            TransformationType.Replace,
            TransformationType.DefaultValue,
            TransformationType.ParsePhone,
            TransformationType.ParseEmail
        };
    }

    public List<string> GetTransformationParameters(string transformationType)
    {
        return transformationType switch
        {
            TransformationType.SplitFullName => new List<string> { "TargetFields" }, // e.g., "FirstName,LastName,MiddleInitial"
            TransformationType.FormatDate => new List<string> { "InputFormat", "OutputFormat" },
            TransformationType.PadLeft => new List<string> { "Length", "PadChar" },
            TransformationType.PadRight => new List<string> { "Length", "PadChar" },
            TransformationType.Substring => new List<string> { "Start", "Length" },
            TransformationType.Replace => new List<string> { "OldValue", "NewValue" },
            TransformationType.DefaultValue => new List<string> { "DefaultValue" },
            _ => new List<string>()
        };
    }

    public bool ValidateTransformation(DataTransformation transformation, out string? errorMessage)
    {
        errorMessage = null;

        if (string.IsNullOrWhiteSpace(transformation.TransformationType))
        {
            errorMessage = "Transformation type is required";
            return false;
        }

        if (!GetAvailableTransformations().Contains(transformation.TransformationType))
        {
            errorMessage = $"Unknown transformation type: {transformation.TransformationType}";
            return false;
        }

        var requiredParams = GetTransformationParameters(transformation.TransformationType);
        foreach (var param in requiredParams)
        {
            if (!transformation.Parameters.ContainsKey(param) || string.IsNullOrWhiteSpace(transformation.Parameters[param]))
            {
                errorMessage = $"Missing required parameter: {param}";
                return false;
            }
        }

        return true;
    }

    public TransformationResult ApplyTransformation(string value, DataTransformation transformation)
    {
        try
        {
            if (!ValidateTransformation(transformation, out var validationError))
            {
                return new TransformationResult
                {
                    Success = false,
                    ErrorMessage = validationError
                };
            }

            return transformation.TransformationType switch
            {
                TransformationType.TrimWhitespace => TransformTrimWhitespace(value),
                TransformationType.UpperCase => TransformUpperCase(value),
                TransformationType.LowerCase => TransformLowerCase(value),
                TransformationType.TitleCase => TransformTitleCase(value),
                TransformationType.RemoveSpecialChars => TransformRemoveSpecialChars(value),
                TransformationType.PadLeft => TransformPadLeft(value, transformation.Parameters),
                TransformationType.PadRight => TransformPadRight(value, transformation.Parameters),
                TransformationType.Substring => TransformSubstring(value, transformation.Parameters),
                TransformationType.Replace => TransformReplace(value, transformation.Parameters),
                TransformationType.DefaultValue => TransformDefaultValue(value, transformation.Parameters),
                TransformationType.FormatDate => TransformFormatDate(value, transformation.Parameters),
                TransformationType.ParsePhone => TransformParsePhone(value),
                TransformationType.ParseEmail => TransformParseEmail(value),
                _ => new TransformationResult
                {
                    Success = false,
                    ErrorMessage = $"Transformation {transformation.TransformationType} not implemented for single value"
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying transformation {Type}", transformation.TransformationType);
            return new TransformationResult
            {
                Success = false,
                ErrorMessage = $"Transformation error: {ex.Message}"
            };
        }
    }

    public TransformationResult ApplyTransformationToRow(Dictionary<string, string> row, string sourceColumn, DataTransformation transformation)
    {
        try
        {
            if (!row.ContainsKey(sourceColumn))
            {
                return new TransformationResult
                {
                    Success = false,
                    ErrorMessage = $"Source column '{sourceColumn}' not found in row"
                };
            }

            var value = row[sourceColumn];

            // Special handling for transformations that produce multiple values
            if (transformation.TransformationType == TransformationType.SplitFullName)
            {
                return TransformSplitFullName(value, transformation.Parameters);
            }

            // For single-value transformations, use the standard method
            return ApplyTransformation(value, transformation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying row transformation {Type}", transformation.TransformationType);
            return new TransformationResult
            {
                Success = false,
                ErrorMessage = $"Transformation error: {ex.Message}"
            };
        }
    }

    #region Transformation Implementations

    private TransformationResult TransformTrimWhitespace(string value)
    {
        return new TransformationResult
        {
            Success = true,
            TransformedValues = new Dictionary<string, string> { { "value", value.Trim() } }
        };
    }

    private TransformationResult TransformUpperCase(string value)
    {
        return new TransformationResult
        {
            Success = true,
            TransformedValues = new Dictionary<string, string> { { "value", value.ToUpperInvariant() } }
        };
    }

    private TransformationResult TransformLowerCase(string value)
    {
        return new TransformationResult
        {
            Success = true,
            TransformedValues = new Dictionary<string, string> { { "value", value.ToLowerInvariant() } }
        };
    }

    private TransformationResult TransformTitleCase(string value)
    {
        var textInfo = CultureInfo.CurrentCulture.TextInfo;
        return new TransformationResult
        {
            Success = true,
            TransformedValues = new Dictionary<string, string> { { "value", textInfo.ToTitleCase(value.ToLower()) } }
        };
    }

    private TransformationResult TransformRemoveSpecialChars(string value)
    {
        var cleaned = Regex.Replace(value, @"[^a-zA-Z0-9\s]", "");
        return new TransformationResult
        {
            Success = true,
            TransformedValues = new Dictionary<string, string> { { "value", cleaned } }
        };
    }

    private TransformationResult TransformPadLeft(string value, Dictionary<string, string> parameters)
    {
        var length = int.Parse(parameters["Length"]);
        var padChar = parameters.GetValueOrDefault("PadChar", " ")[0];
        return new TransformationResult
        {
            Success = true,
            TransformedValues = new Dictionary<string, string> { { "value", value.PadLeft(length, padChar) } }
        };
    }

    private TransformationResult TransformPadRight(string value, Dictionary<string, string> parameters)
    {
        var length = int.Parse(parameters["Length"]);
        var padChar = parameters.GetValueOrDefault("PadChar", " ")[0];
        return new TransformationResult
        {
            Success = true,
            TransformedValues = new Dictionary<string, string> { { "value", value.PadRight(length, padChar) } }
        };
    }

    private TransformationResult TransformSubstring(string value, Dictionary<string, string> parameters)
    {
        var start = int.Parse(parameters["Start"]);
        var length = parameters.ContainsKey("Length") ? int.Parse(parameters["Length"]) : value.Length - start;

        if (start >= value.Length)
        {
            return new TransformationResult
            {
                Success = false,
                ErrorMessage = "Start index exceeds string length"
            };
        }

        var result = value.Substring(start, Math.Min(length, value.Length - start));
        return new TransformationResult
        {
            Success = true,
            TransformedValues = new Dictionary<string, string> { { "value", result } }
        };
    }

    private TransformationResult TransformReplace(string value, Dictionary<string, string> parameters)
    {
        var oldValue = parameters["OldValue"];
        var newValue = parameters["NewValue"];
        return new TransformationResult
        {
            Success = true,
            TransformedValues = new Dictionary<string, string> { { "value", value.Replace(oldValue, newValue) } }
        };
    }

    private TransformationResult TransformDefaultValue(string value, Dictionary<string, string> parameters)
    {
        var defaultValue = parameters["DefaultValue"];
        var result = string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        return new TransformationResult
        {
            Success = true,
            TransformedValues = new Dictionary<string, string> { { "value", result } }
        };
    }

    private TransformationResult TransformFormatDate(string value, Dictionary<string, string> parameters)
    {
        try
        {
            var inputFormat = parameters.GetValueOrDefault("InputFormat", "");
            var outputFormat = parameters["OutputFormat"];

            DateTime date;
            if (string.IsNullOrWhiteSpace(inputFormat))
            {
                // Try to parse with automatic format detection
                if (!DateTime.TryParse(value, out date))
                {
                    return new TransformationResult
                    {
                        Success = false,
                        ErrorMessage = $"Could not parse date: {value}"
                    };
                }
            }
            else
            {
                // Parse with specific format
                if (!DateTime.TryParseExact(value, inputFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    return new TransformationResult
                    {
                        Success = false,
                        ErrorMessage = $"Could not parse date '{value}' with format '{inputFormat}'"
                    };
                }
            }

            return new TransformationResult
            {
                Success = true,
                TransformedValues = new Dictionary<string, string> { { "value", date.ToString(outputFormat) } }
            };
        }
        catch (Exception ex)
        {
            return new TransformationResult
            {
                Success = false,
                ErrorMessage = $"Date formatting error: {ex.Message}"
            };
        }
    }

    private TransformationResult TransformParsePhone(string value)
    {
        // Remove common phone number formatting
        var cleaned = Regex.Replace(value, @"[^\d]", "");
        return new TransformationResult
        {
            Success = true,
            TransformedValues = new Dictionary<string, string> { { "value", cleaned } }
        };
    }

    private TransformationResult TransformParseEmail(string value)
    {
        var trimmed = value.Trim().ToLowerInvariant();
        var isValid = Regex.IsMatch(trimmed, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        if (!isValid)
        {
            return new TransformationResult
            {
                Success = false,
                ErrorMessage = $"Invalid email format: {value}"
            };
        }

        return new TransformationResult
        {
            Success = true,
            TransformedValues = new Dictionary<string, string> { { "value", trimmed } }
        };
    }

    private TransformationResult TransformSplitFullName(string value, Dictionary<string, string> parameters)
    {
        try
        {
            var targetFields = parameters.GetValueOrDefault("TargetFields", "FirstName,LastName").Split(',');
            var parts = value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var result = new Dictionary<string, string>();

            if (targetFields.Contains("FirstName"))
            {
                result["FirstName"] = parts.Length > 0 ? parts[0] : "";
            }

            if (targetFields.Contains("LastName"))
            {
                result["LastName"] = parts.Length > 1 ? parts[^1] : ""; // Last element
            }

            if (targetFields.Contains("MiddleName") || targetFields.Contains("MiddleInitial"))
            {
                var middleName = parts.Length > 2 ? string.Join(" ", parts.Skip(1).Take(parts.Length - 2)) : "";
                var middleInitial = middleName.Length > 0 ? middleName[0].ToString() : "";

                if (targetFields.Contains("MiddleName"))
                {
                    result["MiddleName"] = middleName;
                }
                if (targetFields.Contains("MiddleInitial"))
                {
                    result["MiddleInitial"] = middleInitial;
                }
            }

            return new TransformationResult
            {
                Success = true,
                TransformedValues = result
            };
        }
        catch (Exception ex)
        {
            return new TransformationResult
            {
                Success = false,
                ErrorMessage = $"Name splitting error: {ex.Message}"
            };
        }
    }

    #endregion
}
