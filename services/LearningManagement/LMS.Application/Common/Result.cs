namespace LMS.Application.Common;

/// <summary>
/// Result wrapper for operations
/// </summary>
public class Result<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> Errors { get; set; } = new();

    public static Result<T> Ok(T data) => new() { Success = true, Data = data };
    public static Result<T> Fail(string error) => new() { Success = false, ErrorMessage = error, Errors = new List<string> { error } };
    public static Result<T> Fail(List<string> errors) => new() { Success = false, Errors = errors, ErrorMessage = string.Join(", ", errors) };
}

/// <summary>
/// Result for operations without return data
/// </summary>
public class Result
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> Errors { get; set; } = new();

    public static Result Ok() => new() { Success = true };
    public static Result Fail(string error) => new() { Success = false, ErrorMessage = error, Errors = new List<string> { error } };
    public static Result Fail(List<string> errors) => new() { Success = false, Errors = errors, ErrorMessage = string.Join(", ", errors) };
}
