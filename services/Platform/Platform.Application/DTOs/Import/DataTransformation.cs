namespace Platform.Application.DTOs.Import;

/// <summary>
/// Represents a data transformation to apply during import
/// </summary>
public class DataTransformation
{
    public string TransformationType { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = new();
}

/// <summary>
/// Result of applying a transformation
/// </summary>
public class TransformationResult
{
    public bool Success { get; set; }
    public Dictionary<string, string> TransformedValues { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Built-in transformation types
/// </summary>
public static class TransformationType
{
    public const string SplitFullName = "SplitFullName";
    public const string TrimWhitespace = "TrimWhitespace";
    public const string FormatDate = "FormatDate";
    public const string UpperCase = "UpperCase";
    public const string LowerCase = "LowerCase";
    public const string TitleCase = "TitleCase";
    public const string RemoveSpecialChars = "RemoveSpecialChars";
    public const string PadLeft = "PadLeft";
    public const string PadRight = "PadRight";
    public const string Substring = "Substring";
    public const string Replace = "Replace";
    public const string DefaultValue = "DefaultValue";
    public const string ParsePhone = "ParsePhone";
    public const string ParseEmail = "ParseEmail";
}
