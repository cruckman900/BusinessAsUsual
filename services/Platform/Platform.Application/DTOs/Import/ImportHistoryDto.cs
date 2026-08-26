namespace Platform.Application.DTOs.Import;

/// <summary>
/// DTO for import history list view
/// </summary>
public class ImportHistoryDto
{
    public Guid Id { get; set; }
    public string TableName { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public Guid ImportedBy { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalRows { get; set; }
    public int SuccessfulRows { get; set; }
    public int FailedRows { get; set; }
    public string? FileName { get; set; }
    public TimeSpan? Duration { get; set; }
    public DateTime? RolledBackAt { get; set; }
    public bool CanRollback => Status == "Completed" && RolledBackAt == null && SuccessfulRows > 0;
}

/// <summary>
/// DTO for import history detail view
/// </summary>
public class ImportHistoryDetailDto : ImportHistoryDto
{
    public string? MappingConfiguration { get; set; }
    public List<string> ErrorMessages { get; set; } = new();
}
