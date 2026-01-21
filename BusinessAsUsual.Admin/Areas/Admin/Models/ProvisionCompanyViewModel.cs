using BusinessAsUsual.Core.Modules;

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
    /// 
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
        public List<ModuleDefinition> Modules { get; set; } = new();
    }

}
