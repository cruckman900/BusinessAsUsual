using Platform.Application.DTOs.Import;

namespace Platform.Application.Interfaces;

/// <summary>
/// Service for managing column mapping templates
/// </summary>
public interface IMappingTemplateService
{
    /// <summary>
    /// Get all templates for a company and table
    /// </summary>
    Task<IEnumerable<MappingTemplateDto>> GetTemplatesAsync(Guid companyId, string tableName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a specific template with full configuration
    /// </summary>
    Task<MappingTemplateDetailDto?> GetTemplateByIdAsync(Guid templateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Save a new template or update existing
    /// </summary>
    Task<Guid> SaveTemplateAsync(SaveMappingTemplateDto dto, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a template
    /// </summary>
    Task<bool> DeleteTemplateAsync(Guid templateId, Guid companyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all templates accessible to a user (owned + shared)
    /// </summary>
    Task<IEnumerable<MappingTemplateDto>> GetAccessibleTemplatesAsync(Guid companyId, Guid userId, string tableName, CancellationToken cancellationToken = default);
}
