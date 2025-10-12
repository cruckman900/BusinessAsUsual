using BusinessAsUsual.Admin.Areas.Admin.Models;
using BusinessAsUsual.Admin.Services; // IProvisioningService for backend orchestration
using Microsoft.AspNetCore.Mvc; // MVC controller base

namespace BusinessAsUsual.Admin.Areas.Admin.Controllers.Admin
{
    /// <summary>
    /// Handles company provisioning via admin-facing MVC form.
    /// Streams Smart Commit breadcrumbs and redirects to success modal on completion.
    /// </summary>
    [Route("admin/[controller]")] // ✅ Fix: was "admin/Icontroller" (typo). This maps to /admin/provisioncompany
    [Area("Admin")]
    public class ProvisionCompanyController : Controller
    {
        private readonly IProvisioningService _provisioner;

        /// <summary>
        /// Injects the provisioning service for tenant orchestration.
        /// </summary>
        /// <param name="provisioner">Service responsible for provisioning logic.</param>
        public ProvisionCompanyController(IProvisioningService provisioner)
        {
            _provisioner = provisioner;
        }

        /// <summary>
        /// Displays the provisioning form with an empty Company model.
        /// </summary>
        /// <returns>View with empty form fields.</returns>
        [Area("Admin")]
        [Route("/admin/provisioncompany")]
        [HttpGet]
        public IActionResult Index() => View(new Company());

        /// <summary>
        /// Handles form submission, validates input, and triggers provisioning.
        /// On success, logs Smart Commit and redirects to success view.
        /// </summary>
        /// <param name="company">Form-bound company data.</param>
        /// <returns>Redirect to success view or redisplay form with errors.</returns>
        [HttpPost]
        public async Task<IActionResult> Index([FromBody] Company company)
        {
            if (!ModelState.IsValid || company == null)
            {
                return BadRequest("Invalid input. Please check the payload and try again.");
            }

            var success = await _provisioner.ProvisionTenantAsync(
                company.Name,
                company.AdminEmail,
                company.BillingPlan,
                company.ModulesEnabled.Split(","));

            if (success)
            {
                var commitTag = $"🟢 Provisioned {company.Name} with {company.ModulesEnabled} [{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}]";
                TempData["SmartCommit"] = commitTag;

                // ✅ Return 200 OK for functional test
                return Ok(new { message = "Provisioning successful", commitTag });
            }

            return StatusCode(500, "Provisioning failed due to internal error.");
        }

        /// <summary>
        /// Displays the success view with Smart Commit breadcrumb.
        /// </summary>
        /// <returns>Success view with commit tag.</returns>
        public IActionResult Success()
        {
            ViewBag.CommitTag = TempData["SmartCommit"];
            return View();
        }
    }
}