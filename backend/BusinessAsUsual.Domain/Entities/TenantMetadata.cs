namespace BusinessAsUsual.Domain.Entities;

/// <summary>
/// Stores metadata about provisioned tenant companies.
/// </summary>
public class TenantMetadata
{
    /// <summary>
    /// Internal unique identifier for the tenant metadata entry.
    /// </summary>
    public Guid TenantMetadataID { get; set; }

    /// <summary>
    /// Name of the provisioned company.
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Database name assigned to the tenant.
    /// </summary>
    public string DatabaseName { get; set; } = string.Empty;

    /// <summary>
    /// Database connection string for the tenant.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the tenant was provisioned.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}