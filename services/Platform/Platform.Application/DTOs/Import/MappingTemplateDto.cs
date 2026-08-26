namespace Platform.Application.DTOs.Import;

/// <summary>
/// DTO for listing mapping templates
/// </summary>
public class MappingTemplateDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsShared { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for loading a complete mapping template with configuration
/// </summary>
public class MappingTemplateDetailDto : MappingTemplateDto
{
    public string ConfigurationJson { get; set; } = string.Empty;
}

/// <summary>
/// DTO for creating or updating a mapping template
/// </summary>
public class SaveMappingTemplateDto
{
    public Guid? Id { get; set; }
    public Guid CompanyId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ConfigurationJson { get; set; } = string.Empty;
    public bool IsShared { get; set; }
}
