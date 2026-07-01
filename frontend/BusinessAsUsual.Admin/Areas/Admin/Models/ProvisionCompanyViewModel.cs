using BusinessAsUsual.Core.Modules;
using BusinessAsUsual.Domain.Entities;

namespace BusinessAsUsual.Admin.Areas.Admin.Models
{
    /// <summary>
    /// Represents the data required to provision a company, including company details and associated modules.
    /// </summary>
    public class ProvisionCompanyViewModel
    {
        /// <summary>
        /// Gets or sets the company associated with this instance.
        /// </summary>
        public required Company Company { get; set; }

        /// <summary>
        /// Gets or sets the collection of module groups to be displayed or managed.
        /// </summary>
        public List<ModuleGroupViewModel> GroupedModules { get; set; } = new();
    }

    /// <summary>
    /// Represents a group of modules for display in provisioning UI
    /// </summary>
    public class ModuleGroupViewModel
    {
        /// <summary>
        /// Gets or sets the name of the group associated with this instance.
        /// </summary>
        public string GroupName { get; set; } = "";

        /// <summary>
        /// Gets or sets the collection of module definitions associated with this instance.
        /// </summary>
        public List<SelectableModuleDefinition> Modules { get; set; } = new();
    }

    /// <summary>
    /// Wrapper for ModuleDefinition that adds selection state for UI
    /// </summary>
    public class SelectableModuleDefinition
    {
        /// <summary>
        /// Gets or sets the module group.
        /// </summary>
        public string Group { get; set; } = "";

        /// <summary>
        /// Gets or sets the module key identifier.
        /// </summary>
        public string Key { get; set; } = "";

        /// <summary>
        /// Gets or sets the module display name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets the list of selectable submodules.
        /// </summary>
        public List<SelectableSubmoduleDefinition> Submodules { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether this module is selected.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Creates a SelectableModuleDefinition from a ModuleDefinition.
        /// </summary>
        /// <param name="module">The module definition to convert.</param>
        /// <returns>A new SelectableModuleDefinition instance.</returns>
        public static SelectableModuleDefinition FromModuleDefinition(ModuleDefinition module)
        {
            return new SelectableModuleDefinition
            {
                Group = module.Group,
                Key = module.Key,
                Name = module.Name,
                Submodules = module.Submodules.Select(SelectableSubmoduleDefinition.FromSubmoduleDefinition).ToList()
            };
        }
    }

    /// <summary>
    /// Wrapper for SubmoduleDefinition that adds selection state for UI
    /// </summary>
    public class SelectableSubmoduleDefinition
    {
        /// <summary>
        /// Gets or sets the submodule key identifier.
        /// </summary>
        public string Key { get; set; } = "";

        /// <summary>
        /// Gets or sets the submodule display name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets a value indicating whether this submodule is selected.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Creates a SelectableSubmoduleDefinition from a SubmoduleDefinition.
        /// </summary>
        /// <param name="submodule">The submodule definition to convert.</param>
        /// <returns>A new SelectableSubmoduleDefinition instance.</returns>
        public static SelectableSubmoduleDefinition FromSubmoduleDefinition(SubmoduleDefinition submodule)
        {
            return new SelectableSubmoduleDefinition
            {
                Key = submodule.Key,
                Name = submodule.Name
            };
        }
    }
}
