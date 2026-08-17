using MudBlazor;

namespace BusinessAsUsual.Web.Modules._Shared
{
    /// <summary>
    /// Serves as the base class for LMS (Learning Management System) modules, providing common configuration 
    /// and metadata for learning and training functionality.
    /// </summary>
    /// <remarks>
    /// Inherit from this class to implement modules that manage LMS features such as courses, 
    /// certifications, learner progress, and training assignments.
    /// </remarks>
    public abstract class LMSLandingBase : ModuleBase
    {
        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            ModuleName = "Learning";
            ModuleDescription = "Training courses, certifications, and learning paths.";
            ModuleIcon = Icons.Material.Filled.School;
        }

        /// <summary>
        /// Gets the root route path for the LMS module.
        /// </summary>
        protected override string ModuleRootRoute => "/lms";
    }
}
