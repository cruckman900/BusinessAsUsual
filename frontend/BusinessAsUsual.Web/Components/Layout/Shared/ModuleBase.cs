using Microsoft.AspNetCore.Components;
using MudBlazor;

/// <summary>
/// Serves as a base class for dashboard modules, providing common properties and functionality for module components.
/// </summary>
/// <remarks>Inherit from this class to implement custom dashboard modules with standardized naming, description,
/// icon, and breadcrumb navigation support. This class is intended for use within a component-based dashboard
/// framework.</remarks>
public abstract class ModuleBase : ComponentBase
{
    /// <summary>
    /// Gets or sets the name of the module associated with the current instance.
    /// </summary>
    protected string ModuleName { get; set; } = "";

    /// <summary>
    /// Gets or sets the description of the module.
    /// </summary>
    protected string? ModuleDescription { get; set; }

    /// <summary>
    /// Gets or sets the icon associated with the module.
    /// </summary>
    protected string? ModuleIcon { get; set; }

    /// <summary>
    /// Gets the root route path for the module.
    /// </summary>
    /// <remarks>Override this property in a derived class to specify a different base route for the module's
    /// endpoints.</remarks>
    protected virtual string ModuleRootRoute => "/dashboard";

    /// <summary>
    /// Builds a list of breadcrumb items representing the navigation path for the current page.
    /// </summary>
    /// <param name="pageTitle">The title of the current page to display as the final breadcrumb. If null, the module name is used.</param>
    /// <returns>A list of <see cref="BreadcrumbItem"/> objects representing the breadcrumb trail for the current page.</returns>
    protected virtual List<BreadcrumbItem> BuildBreadCrumbs(string? pageTitle = null)
    {
        return
        [
            new BreadcrumbItem("Dashboard", "/dashboard"),
            //new BreadcrumbItem(ModuleName, $"{ModuleRootRoute}/{ModuleName.ToLower()}", false),
            new BreadcrumbItem(pageTitle ?? ModuleName, null, true)
        ];

    }
}