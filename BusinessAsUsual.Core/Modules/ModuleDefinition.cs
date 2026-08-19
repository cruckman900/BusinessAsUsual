namespace BusinessAsUsual.Core.Modules
{
    /// <summary>
    /// Represents the definition of a module, including its unique identifier, group, key, name, and associated submodules.
    /// </summary>
    /// <param name="Id">Unique identifier for the module.</param>
    /// <param name="Group">Module group/category (e.g., "Platform", "HR", "Sales").</param>
    /// <param name="Key">Short key identifier for the module (e.g., "hr", "crm", "finance").</param>
    /// <param name="Name">Display name for the module (e.g., "Human Resources", "CRM").</param>
    /// <param name="Submodules">List of submodules within this module.</param>
    public record ModuleDefinition(
        Guid Id,
        string Group,
        string Key,
        string Name,
        IReadOnlyList<SubmoduleDefinition> Submodules
    );
}
