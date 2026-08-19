namespace BusinessAsUsual.Core.Modules
{
    /// <summary>
    /// Represents the definition of a submodule within a module.
    /// </summary>
    /// <param name="Id">Unique identifier for the submodule.</param>
    /// <param name="Key">Short key identifier for the submodule (e.g., "Employees", "Leads").</param>
    /// <param name="Name">Display name for the submodule (e.g., "Employee Management", "Lead Tracking").</param>
    public record SubmoduleDefinition(
        Guid Id,
        string Key,
        string Name
    );
}
