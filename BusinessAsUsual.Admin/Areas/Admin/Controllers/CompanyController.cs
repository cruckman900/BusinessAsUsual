using BusinessAsUsual.Admin.Areas.Admin.Models;
using BusinessAsUsual.Admin.Data;
using BusinessAsUsual.Admin.Hubs;
using BusinessAsUsual.Admin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;


namespace BusinessAsUsual.Admin.Areas.Admin.Controllers
{
    /// <summary>
    /// Handles all company-related admin actions such as provisioning, viewing, editing, and auditing.
    /// </summary>
    [Area("Admin")]
    [Route("admin/company")]
    public class CompanyController : Controller
    {
        private readonly IProvisioningService _provisioner;
        private readonly IHubContext<SmartCommitHub> _hubContext;
        private readonly AdminDbContext _context;

        /// <summary>
        /// Injects the provisioning service for tenant orchestration.
        /// </summary>
        /// <param name="provisioningService">Service responsible for provisioning logic.</param>
        /// <param name="hubContext"></param>
        /// <param name="context"></param>
        public CompanyController(IProvisioningService provisioningService, IHubContext<SmartCommitHub> hubContext, AdminDbContext context)
        {
            _provisioner = provisioningService;
            _hubContext = hubContext;
            _context = context;
        }

        /// <summary>
        /// Displays the company provisioning form.
        /// </summary>
        [HttpGet("provision")]
        public IActionResult ProvisionCompany() => View(new Company());

        /// <summary>
        /// Handles form submission, validates input, and triggers provisioning.
        /// On success, logs Smart Commit and redirects to success view.
        /// </summary>
        /// <param name="company">Form-bound company data.</param>
        /// <returns>Redirect to success view or redisplay form with errors.</returns>
        [HttpPost("provision")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ProvisionCompany([FromForm] Company company)
        {
            if (!ModelState.IsValid || company == null)
            {
                return View(company); // redisplay form with validation errors
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

                await _hubContext.Clients.All.SendAsync("ReceiveCommit", commitTag);

                if (Request.Headers.Accept.ToString().Contains("application/json"))
                {
                    return Ok(new { message = "Provisioning successful", commitTag });
                }
                else
                {
                    return RedirectToAction("ProvisionSuccess");
                }
            }

            ModelState.AddModelError("", "Provisioning failed due to internal error.");
            return View(company);
        }

        /// <summary>
        /// Displays the success view with Smart Commit breadcrumb.
        /// </summary>
        /// <returns>Success view with commit tag.</returns>
        [HttpGet("provision/success")]
        public IActionResult ProvisionSuccess()
        {
            ViewBag.CommitTag = TempData["SmartCommit"];
            return View();
        }

        /// <summary>
        /// Lists all companies with filtering options.
        /// </summary>
        [HttpGet("view")]
        public IActionResult ViewCompanies()
        {
            var companies = _context.Companies.ToList(); // Pull from DB

            var displayNames = typeof(Company)
                .GetProperties()
                .ToDictionary(
                    prop => prop.Name.ToLowerInvariant(),
                    prop => prop.GetCustomAttribute<DisplayAttribute>()?.Name ?? prop.Name
                );

            ViewData["GridId"] = "companies-grid";
            ViewData["ColumnDisplayNames"] = displayNames;

            return View(companies.Cast<dynamic>().ToList());
        }

        /// <summary>
        /// Displays the edit form for a specific company.
        /// </summary>
        /// <param name="id">The unique identifier of the company.</param>
        [HttpGet("edit/{id}")]
        public IActionResult EditCompany(Guid id) => View();

        /// <summary>
        /// Shows detailed metadata for a specific company.
        /// </summary>
        /// <param name="id">The unique identifier of the company.</param>
        [HttpGet("details")]
        public async Task<IActionResult> CompanyDetails(Guid id)
        {
            var company = await _context.Companies.FindAsync(id);

            if (ModelState.IsValid) return View(company);
            else return NotFound();
        }

        /// <summary>
        /// Displays the archive confirmation for a company.
        /// </summary>
        /// <param name="id">The unique identifier of the company.</param>
        [HttpGet("archive/{id}")]
        public IActionResult ArchiveCompany(Guid id) => View();

        /// <summary>
        /// Allows merging of two companies.
        /// </summary>
        [HttpGet("merge")]
        public IActionResult MergeCompanies() => View();

        /// <summary>
        /// Displays the audit log for company-related actions.
        /// </summary>
        [HttpGet("audit")]
        public IActionResult AuditLog() => View();

        /// <summary>
        /// Shows company-specific configuration options.
        /// </summary>
        [HttpGet("settings")]
        public IActionResult CompanySettings() => View();
    }
}