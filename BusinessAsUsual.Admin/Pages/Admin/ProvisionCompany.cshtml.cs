using BusinessAsUsual.Admin.Models;
using BusinessAsUsual.Admin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace BusinessAsUsual.Admin.Pages.Admin
{
    /// <summary>
    /// Razor PageModel for provisioning a new company via the admin interface.
    /// </summary>
    public class ProvisionCompanyModel : PageModel
    {
        private readonly CompanyProvisioner _provisioner;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProvisionCompanyModel"/> class.
        /// </summary>
        /// <param name="provisioner">The service responsible for provisioning company databases and metadata.</param>
        public ProvisionCompanyModel(CompanyProvisioner provisioner)
        {
            _provisioner = provisioner ?? throw new ArgumentNullException(nameof(provisioner));
        }

        /// <summary>
        /// Gets or sets the company being provisioned. Bound to the form input.
        /// </summary>
        [BindProperty]
        public Company Company { get; set; } = new Company();

        /// <summary>
        /// Handles POST requests to provision a new company.
        /// </summary>
        /// <returns>Redirects to the success page if provisioning succeeds; otherwise, reloads the current page with error feedback.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || Company == null)
            {
                ModelState.AddModelError("", "Invalid input. Please check the form and try again.");
                return Page();
            }

            var success = await _provisioner.CreateCompanyDatabaseAsync(Company.Name, Company.AdminEmail);

            if (success)
            {
                return RedirectToPage("Success");
            }

            ModelState.AddModelError("", "Provisioning failed. Please try again.");
            return Page();
        }
    }
}