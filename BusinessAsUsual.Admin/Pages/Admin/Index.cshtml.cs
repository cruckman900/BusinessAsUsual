using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BusinessAsUsual.Admin.Pages.Admin
{
    /// <summary>
    /// Admin dashboard page for provisioning new companies.
    /// </summary>
    public class IndexModel : PageModel
    {
        /// <summary>
        /// The name of the company to be provisioned.
        /// </summary>
        [BindProperty] public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// The admin email for the new company.
        /// </summary>
        [BindProperty] public string AdminEmail { get; set; } = string.Empty;

        /// <summary>
        /// Handles the form submission to provision a new company.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            // TODO: Provisioning logic goes here
            await Task.CompletedTask;

            TempData["Success"] = $"Company '{CompanyName}' provisioned successfully! 🛠️ More cowbell coming soon...";
            return RedirectToPage();
        }
    }
}