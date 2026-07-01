namespace BusinessAsUsual.Core.Modules
{
    /// <summary>
    /// Represents the definition of a module, including its group, key, name, and associated submodules.
    /// </summary>
    /// <param name="Group"></param>
    /// <param name="Key"></param>
    /// <param name="Name"></param>
    /// <param name="Submodules"></param>
    public record ModuleDefinition(
        string Group,
        string Key,
        string Name,
        IReadOnlyList<SubmoduleDefinition> Submodules
    );
}
