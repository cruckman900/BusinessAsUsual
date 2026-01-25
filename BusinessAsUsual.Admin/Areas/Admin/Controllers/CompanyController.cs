using BusinessAsUsual.Core.Modules;
using BusinessAsUsual.Admin.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using BusinessAsUsual.Application.Services.Provisioning;
using BusinessAsUsual.Application.Contracts;
using BusinessAsUsual.Infrastructure.Persistence;
using BusinessAsUsual.Admin.Areas.Admin.Models;

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
        private readonly BusinessDbContext _context;

        /// <summary>
        /// Injects the provisioning service for tenant orchestration.
        /// </summary>
        /// <param name="provisioningService">Service responsible for provisioning logic.</param>
        /// <param name="hubContext"></param>
        /// <param name="context"></param>
        public CompanyController(IProvisioningService provisioningService, IHubContext<SmartCommitHub> hubContext, BusinessDbContext context)
        {
            _provisioner = provisioningService;
            _hubContext = hubContext;
            _context = context;
        }

        /// <summary>
        /// Displays the company provisioning form.
        /// </summary>
        [HttpGet("provision")]
        public IActionResult ProvisionCompany()
        {
            var vm = new ProvisionCompanyViewModel
            {
                Company = new Company(),
                GroupedModules = ModuleCatalog.AllModules
                    .GroupBy(m => m.Group)
                    .Select(g => new ModuleGroupViewModel
                    {
                        GroupName = g.Key,
                        Modules = g.ToList()
                    })
                    .ToList()
            };

            return View(vm);
        }

        /// <summary>
        /// Handles the HTTP POST request to provision a new company tenant based on the provided view model.
        /// </summary>
        /// <remarks>If the model state is invalid or provisioning fails, the method returns the same view
        /// with error messages. On successful provisioning, the user is redirected to a confirmation page. This action
        /// expects to be called from a form submission.</remarks>
        /// <param name="vm">The view model containing the company details and provisioning options to be used for tenant creation. Must
        /// not be null and must contain valid data.</param>
        /// <returns>An <see cref="IActionResult"/> that renders the provisioning view with validation errors if the input is
        /// invalid or provisioning fails; otherwise, a redirect to the provisioning success page.</returns>
        [HttpPost("provision")]
        public async Task<IActionResult> ProvisionCompany(ProvisionCompanyViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var company = vm.Company;

            var modules = (company.ModulesEnabled ?? "")
                .Split(",", StringSplitOptions.RemoveEmptyEntries);

            var submodules = (company.SubmodulesEnabled ?? "")
                .Split(",", StringSplitOptions.RemoveEmptyEntries);

            var request = new ProvisioningRequest
            {
                CompanyName = company.Name,
                AdminEmail = company.AdminEmail,
                BillingPlan = company.BillingPlan,
                Modules = modules,
                Submodules = submodules
            };

            var result = await _provisioner.ProvisionTenantAsync(request);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Error ?? "Provisioning failed.");
                return View(vm);
            }

            TempData["SmartCommit"] = $"Provisioned {company.Name}";
            return RedirectToAction("ProvisionSuccess");
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