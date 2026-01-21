namespace BusinessAsUsual.Core.Modules
{
    public record ModuleDefinition(
        string Group,
        string Key,
        string Name,
        IReadOnlyList<SubmoduleDefinition> Submodules
    );
}
