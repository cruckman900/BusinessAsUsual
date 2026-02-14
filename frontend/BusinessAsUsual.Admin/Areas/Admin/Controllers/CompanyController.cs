using BusinessAsUsual.Admin.Areas.Admin.Models;
using BusinessAsUsual.Admin.Hubs;
using BusinessAsUsual.Application.Contracts;
using BusinessAsUsual.Core.Modules;
using BusinessAsUsual.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BusinessAsUsual.Admin.Areas.Admin.Controllers
{
    /// <summary>
    /// Provides administrative actions for managing companies, including provisioning, editing, archiving, merging, and
    /// viewing company details within the admin area.
    /// </summary>
    /// <remarks>This controller is intended for use within the administrative section of the application and
    /// exposes endpoints for company lifecycle management. Most actions require appropriate administrative permissions.
    /// API calls to external services are performed using an HTTP client factory. Some endpoints are UI-only and do not
    /// interact with backend APIs.</remarks>
    [Area("Admin")]
    [Route("admin/company")]
    public class CompanyController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHubContext<SmartCommitHub> _hubContext;

        /// <summary>
        /// Initializes a new instance of the CompanyController class with the specified HTTP client factory and SignalR
        /// hub context.
        /// </summary>
        /// <param name="httpClientFactory">The factory used to create HttpClient instances for making HTTP requests.</param>
        /// <param name="hubContext">The SignalR hub context used to communicate with connected clients via the SmartCommitHub.</param>
        public CompanyController(
            IHttpClientFactory httpClientFactory,
            IHubContext<SmartCommitHub> hubContext)
        {
            _httpClientFactory = httpClientFactory;
            _hubContext = hubContext;
        }

        // ------------------------------------------------------------
        // GET: Provision form
        // ------------------------------------------------------------
        /// <summary>
        /// Handles HTTP GET requests for the company provisioning form. Prepares and returns a view that allows users
        /// to configure and provision a new company.
        /// </summary>
        /// <remarks>The returned view model includes a new company instance and a list of available
        /// modules grouped by category. This endpoint is typically used to display the initial provisioning form before
        /// any company data has been submitted.</remarks>
        /// <returns>A view result that renders the company provisioning form with the necessary data for module selection.</returns>
        [HttpGet("provision")]
        public IActionResult ProvisionCompany()
        {
            var vm = new ProvisionCompanyViewModel
            {
                Company = new Company(), // change this to a viewmodel?
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

        // ------------------------------------------------------------
        // POST: Provision company (calls API)
        // ------------------------------------------------------------
        /// <summary>
        /// Handles a POST request to provision a new company by submitting the provided company details to the
        /// provisioning API.
        /// </summary>
        /// <remarks>If the model state is invalid or the provisioning API returns an error, the method
        /// redisplays the form with appropriate error messages. On successful provisioning, the user is redirected to a
        /// confirmation page. This action is intended to be called from a form submission in the UI.</remarks>
        /// <param name="vm">A view model containing the company information to be provisioned. Must not be null and must contain valid
        /// data.</param>
        /// <returns>An <see cref="IActionResult"/> that renders the provisioning form with validation errors if the input is
        /// invalid or provisioning fails; otherwise, a redirect to the provisioning success page.</returns>
        [HttpPost("provision")]
        public async Task<IActionResult> ProvisionCompany(ProvisionCompanyViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var company = vm.Company;

            var request = new ProvisioningRequest
            {
                CompanyName = company.Name,
                AdminEmail = company.AdminEmail,
                BillingPlan = company.BillingPlan,
                Modules = (company.ModulesEnabled ?? "")
                    .Split(",", StringSplitOptions.RemoveEmptyEntries),
                Submodules = (company.SubmodulesEnabled ?? "")
                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
            };

            var client = _httpClientFactory.CreateClient("ProvisioningApi");

            var response = await client.PostAsJsonAsync(
                "/api/provisioning/provision-company",
                request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Provisioning failed: {error}");
                return View(vm);
            }

            var result = await response.Content.ReadFromJsonAsync<ProvisioningResult>();

            if (result is null || !result.Success)
            {
                ModelState.AddModelError("", result?.Error ?? "Provisioning failed.");
                return View(vm);
            }

            TempData["SmartCommit"] = $"Provisioned {company.Name}";
            return RedirectToAction("ProvisionSuccess");
        }

        // ------------------------------------------------------------
        // GET: Provision success
        // ------------------------------------------------------------
        /// <summary>
        /// Handles GET requests for the provision success page and returns the corresponding view.
        /// </summary>
        /// <returns>A view result that renders the provision success page.</returns>
        [HttpGet("provision/success")]
        public IActionResult ProvisionSuccess()
        {
            ViewBag.CommitTag = TempData["SmartCommit"];
            return View();
        }

        // ------------------------------------------------------------
        // GET: List companies (calls API)
        // ------------------------------------------------------------
        //[HttpGet("view")]
        //public async Task<IActionResult> ViewCompanies()
        //{
        //    var client = _httpClientFactory.CreateClient("ProvisioningApi");

        //    var companies = await client.GetFromJsonAsync<List<CompanyListItemDto>>(
        //        "/api/companies");

        //    if (companies is null)
        //        companies = new List<CompanyListItemDto>();

        //    // Build display names dynamically
        //    var displayNames = typeof(CompanyListItemDto)
        //        .GetProperties()
        //        .ToDictionary(
        //            prop => prop.Name.ToLowerInvariant(),
        //            prop => prop.GetCustomAttribute<DisplayAttribute>()?.Name ?? prop.Name
        //        );

        //    ViewData["GridId"] = "companies-grid";
        //    ViewData["ColumnDisplayNames"] = displayNames;

        //    return View(companies.Cast<dynamic>().ToList());
        //}

        // ------------------------------------------------------------
        // GET: Company details (calls API)
        // ------------------------------------------------------------
        //[HttpGet("details/{id:guid}")]
        //public async Task<IActionResult> CompanyDetails(Guid id)
        //{
        //    var client = _httpClientFactory.CreateClient("ProvisioningApi");

        //    var company = await client.GetFromJsonAsync<CompanyDetailsDto>(
        //        $"/api/companies/{id}");

        //    if (company is null)
        //        return NotFound();

        //    return View(company);
        //}

        // ------------------------------------------------------------
        // GET: Edit company (UI only)
        // ------------------------------------------------------------
        /// <summary>
        /// Displays the edit form for the specified company.
        /// </summary>
        /// <remarks>This action only renders the edit form and does not perform any data retrieval or
        /// modification. The company data should be loaded by the view or client-side logic as needed.</remarks>
        /// <param name="id">The unique identifier of the company to edit.</param>
        /// <returns>A view that displays the edit form for the company.</returns>
        [HttpGet("edit/{id:guid}")]
        public IActionResult EditCompany(Guid id) => View();

        // ------------------------------------------------------------
        // GET: Archive company (UI only)
        // ------------------------------------------------------------
        /// <summary>
        /// Displays the archive confirmation view for the specified company.
        /// </summary>
        /// <param name="id">The unique identifier of the company to be archived.</param>
        /// <returns>A view that allows the user to confirm archiving the specified company.</returns>
        [HttpGet("archive/{id:guid}")]
        public IActionResult ArchiveCompany(Guid id) => View();

        // ------------------------------------------------------------
        // GET: Merge companies (UI only)
        // ------------------------------------------------------------
        /// <summary>
        /// Returns the view for merging companies in the user interface.
        /// </summary>
        /// <returns>A view result that renders the merge companies page.</returns>
        [HttpGet("merge")]
        public IActionResult MergeCompanies() => View();

        // ------------------------------------------------------------
        // GET: Audit log (calls API)
        // ------------------------------------------------------------
        //[HttpGet("audit/{id:guid}")]
        //public async Task<IActionResult> AuditLog(Guid id)
        //{
        //    var client = _httpClientFactory.CreateClient("ProvisioningApi");

        //    var logs = await client.GetFromJsonAsync<List<AuditLogEntryDto>>(
        //        $"/api/audit/company/{id}");

        //    return View(logs ?? new List<AuditLogEntryDto>());
        //}

        // ------------------------------------------------------------
        // GET: Company settings (calls API)
        // ------------------------------------------------------------
        //[HttpGet("settings/{id:guid}")]
        //public async Task<IActionResult> CompanySettings(Guid id)
        //{
        //    var client = _httpClientFactory.CreateClient("ProvisioningApi");

        //    var settings = await client.GetFromJsonAsync<CompanySettingsDto>(
        //        $"/api/settings/company/{id}");

        //    return View(settings);
        //}
    }
}