namespace BusinessAsUsual.Domain.Models
{
    /// <summary>
    /// Represents the root module configuration for a tenant, containing all enabled modules and submodules.
    /// This model is serialized to JSON and stored in both the master database (Companies.ModuleConfiguration)
    /// and tenant databases (ModuleRegistry.ModuleConfiguration).
    /// </summary>
    public class ModuleConfigurationRoot
    {
        /// <summary>
        /// List of modules configured for the tenant.
        /// </summary>
        public List<ModuleItem> Modules { get; set; } = new();

        /// <summary>
        /// Configuration schema version for future migration support.
        /// </summary>
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// Timestamp of last configuration update.
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Represents a single module with its enabled/disabled state and associated submodules.
    /// </summary>
    public class ModuleItem
    {
        /// <summary>
        /// Unique identifier for the module.
        /// </summary>
        public Guid ModuleId { get; set; }

        /// <summary>
        /// Display name for the module (e.g., "Human Resources", "CRM").
        /// </summary>
        public string ModuleName { get; set; } = string.Empty;

        /// <summary>
        /// Module group/category (e.g., "HR", "Sales", "Operations", "Platform").
        /// </summary>
        public string Group { get; set; } = string.Empty;

        /// <summary>
        /// Whether this module is enabled for the tenant.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Whether the module's schema has been provisioned (lazy-loaded).
        /// </summary>
        public bool IsProvisioned { get; set; }

        /// <summary>
        /// List of submodules within this module.
        /// </summary>
        public List<SubmoduleItem> Submodules { get; set; } = new();
    }

    /// <summary>
    /// Represents a single submodule within a module.
    /// </summary>
    public class SubmoduleItem
    {
        /// <summary>
        /// Unique identifier for the submodule.
        /// </summary>
        public Guid SubmoduleId { get; set; }

        /// <summary>
        /// Display name for the submodule (e.g., "Employee Management", "Leads").
        /// </summary>
        public string SubmoduleName { get; set; } = string.Empty;

        /// <summary>
        /// Whether this submodule is enabled for the tenant.
        /// </summary>
        public bool Enabled { get; set; }
    }
}
