using MudBlazor;

namespace BusinessAsUsual.Web.Modules._Shared
{
    /// <summary>
    /// Serves as the base class for HR-related modules, providing common configuration and metadata for human resources
    /// functionality.
    /// </summary>
    /// <remarks>Inherit from this class to implement modules that manage HR features such as employee
    /// records, onboarding, and benefits. This class sets default values for module name, description, and icon, and
    /// defines the root route for HR modules.</remarks>
    public abstract class HRLandingBase : ModuleBase
    {
        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            ModuleName = "HR";
            ModuleDescription = "Manage employees, onboarding, benefits, and more.";
            ModuleIcon = Icons.Material.Filled.People;
        }

        /// <summary>
        /// Gets the root route path for the module.
        /// </summary>
        protected override string ModuleRootRoute => "/hr";
    }
}