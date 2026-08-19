using BusinessAsUsual.Domain.Entities;

namespace BusinessAsUsual.Application.Database
{
    /// <summary>
    /// Defines methods for provisioning and managing company-specific databases, including creation, schema
    /// application, and company information persistence.
    /// </summary>
    /// <remarks>Implementations of this interface are responsible for ensuring that the master database
    /// exists, creating new databases for companies, applying schema scripts, and saving company information. Methods
    /// are asynchronous and may involve network or I/O operations. Thread safety and transactional guarantees depend on
    /// the specific implementation.</remarks>
    public interface IProvisioningDb
    {
        /// <summary>
        /// Ensures that the master database exists, creating it if necessary.
        /// </summary>
        /// <remarks>This method is typically called during application startup to guarantee that the
        /// master database is available before performing further operations. If the database already exists, no
        /// changes are made.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task completes when the master database has been
        /// verified or created.</returns>
        Task EnsureMasterDatabaseExistsAsync();

        /// <summary>
        /// Asynchronously creates a new database with the specified name.
        /// </summary>
        /// <param name="name">The name of the database to create. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task completes when the database has been created.</returns>
        Task CreateTenantDatabaseAsync(string name);

        /// <summary>
        /// Applies the specified schema script to the tenant database asynchronously.
        /// </summary>
        /// <param name="db">The name or identifier of the tenant database to which the schema script will be applied. Cannot be null or
        /// empty.</param>
        /// <param name="script">The SQL script that defines the schema changes to apply. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ApplyTenantSchemaAsync(string db, string script);

        /// <summary>
        /// Asynchronously saves the specified company information to the data store.
        /// </summary>
        /// <param name="company">The company entity containing the information to be saved. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task SaveCompanyInfoAsync(Company company);

        /// <summary>
        /// Saves the module configuration JSON to the tenant's ModuleRegistry table.
        /// </summary>
        /// <param name="tenantDbName">Name of the tenant database.</param>
        /// <param name="companyId">Unique identifier of the company.</param>
        /// <param name="moduleConfigJson">JSON string containing the module configuration.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task SaveModuleConfigurationToTenantAsync(string tenantDbName, Guid companyId, string moduleConfigJson);

        /// <summary>
        /// Retrieves the module configuration JSON for a specific tenant.
        /// </summary>
        /// <param name="tenantDbName">Name of the tenant database.</param>
        /// <param name="companyId">Unique identifier of the company.</param>
        /// <returns>The module configuration JSON string, or null if not found.</returns>
        Task<string?> GetModuleConfigurationAsync(string tenantDbName, Guid companyId);
    }
}
