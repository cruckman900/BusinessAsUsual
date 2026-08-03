using BusinessAsUsual.Admin.Areas.Admin.Models;
using BusinessAsUsual.Admin.Attributes;
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
    [AdminAuth]
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
                        Modules = g.Select(SelectableModuleDefinition.FromModuleDefinition).ToList()
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
            {
                // Repopulate module list on validation failure
                vm.GroupedModules = ModuleCatalog.AllModules
                    .GroupBy(m => m.Group)
                    .Select(g => new ModuleGroupViewModel
                    {
                        GroupName = g.Key,
                        Modules = g.Select(SelectableModuleDefinition.FromModuleDefinition).ToList()
                    })
                    .ToList();
                return View(vm);
            }

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

            try
            {
                var client = _httpClientFactory.CreateClient("ProvisioningApi");

                // Use 2-second timeout for faster fallback to mock
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                var response = await client.PostAsJsonAsync(
                    "/api/provisioning/provision-company",
                    request,
                    cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ProvisioningResult>();

                    if (result is not null && result.Success)
                    {
                        TempData["SmartCommit"] = $"Provisioned company: {company.Name}";
                        TempData["CompanyId"] = result.CompanyId;
                        TempData["CompanyName"] = company.Name;
                        return RedirectToAction("ProvisionSuccess");
                    }

                    ModelState.AddModelError("", result?.Error ?? "Provisioning failed.");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Provisioning failed: {error}");
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
            {
                // API unavailable or timeout - fallback to mock success for development
                TempData["SmartCommit"] = $"Provisioned company (mock): {company.Name}";
                TempData["CompanyId"] = Guid.NewGuid().ToString();
                TempData["CompanyName"] = company.Name;
                TempData["MockMode"] = true;
                return RedirectToAction("ProvisionSuccess");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }

            // If we got here, something went wrong - redisplay form
            vm.GroupedModules = ModuleCatalog.AllModules
                .GroupBy(m => m.Group)
                .Select(g => new ModuleGroupViewModel
                {
                    GroupName = g.Key,
                    Modules = g.Select(SelectableModuleDefinition.FromModuleDefinition).ToList()
                })
                .ToList();
            return View(vm);
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
        // GET: View Companies (list/manage page)
        // ------------------------------------------------------------
        /// <summary>
        /// Displays a list of companies with filtering options.
        /// </summary>
        /// <param name="searchQuery">Optional search query to filter companies by name.</param>
        /// <param name="statusFilter">Filter by status (All, Active, Archived). Defaults to Active.</param>
        /// <param name="billingPlanFilter">Optional filter by billing plan.</param>
        /// <returns>A view that displays the filtered list of companies.</returns>
        [HttpGet("view")]
        public async Task<IActionResult> ViewCompanies(
            string? searchQuery = null,
            string? statusFilter = "Active",
            string? billingPlanFilter = null)
        {
            try
            {
                // Try to call the API
                var client = _httpClientFactory.CreateClient("ProvisioningApi");
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(searchQuery))
                    queryParams.Add($"search={Uri.EscapeDataString(searchQuery)}");
                if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
                    queryParams.Add($"status={Uri.EscapeDataString(statusFilter)}");
                if (!string.IsNullOrEmpty(billingPlanFilter) && billingPlanFilter != "All")
                    queryParams.Add($"billingPlan={Uri.EscapeDataString(billingPlanFilter)}");

                var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";

                // Use 2-second timeout for faster fallback
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                var response = await client.GetAsync($"/api/admin/companies{queryString}", cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var apiVm = await response.Content.ReadFromJsonAsync<ViewCompaniesViewModel>();
                    if (apiVm != null)
                    {
                        return View(apiVm);
                    }
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
            {
                // API unavailable or timeout - fall through to mock data
            }

            // Fallback to mock data
            var vm = GetMockCompaniesViewModel(searchQuery, statusFilter, billingPlanFilter);
            return View(vm);
        }

        // ------------------------------------------------------------
        // Helper: Get mock companies (temporary - will be replaced with API call)
        // ------------------------------------------------------------
        /// <summary>
        /// Returns a stubbed list of companies for development purposes.
        /// </summary>
        private ViewCompaniesViewModel GetMockCompaniesViewModel(
            string? searchQuery,
            string? statusFilter,
            string? billingPlanFilter)
        {
            var allCompanies = new List<CompanyListItemViewModel>
            {
                new CompanyListItemViewModel
                {
                    CompanyId = Guid.NewGuid().ToString(),
                    Name = "Acme Corporation",
                    AdminEmail = "admin@acme.com",
                    BillingPlan = "Enterprise",
                    CreatedAt = DateTime.UtcNow.AddMonths(-12),
                    IsArchived = false,
                    ModulesEnabled = "CRM, Inventory, Sales, Finance",
                    UserCount = 45
                },
                new CompanyListItemViewModel
                {
                    CompanyId = Guid.NewGuid().ToString(),
                    Name = "TechStart Inc.",
                    AdminEmail = "admin@techstart.io",
                    BillingPlan = "Professional",
                    CreatedAt = DateTime.UtcNow.AddMonths(-6),
                    IsArchived = false,
                    ModulesEnabled = "CRM, HR",
                    UserCount = 12
                },
                new CompanyListItemViewModel
                {
                    CompanyId = Guid.NewGuid().ToString(),
                    Name = "Global Solutions Ltd",
                    AdminEmail = "contact@globalsolutions.com",
                    BillingPlan = "Enterprise",
                    CreatedAt = DateTime.UtcNow.AddMonths(-18),
                    IsArchived = false,
                    ModulesEnabled = "CRM, Inventory, Sales, Finance, HR",
                    UserCount = 120
                },
                new CompanyListItemViewModel
                {
                    CompanyId = Guid.NewGuid().ToString(),
                    Name = "Startup Ventures",
                    AdminEmail = "hello@startupventures.co",
                    BillingPlan = "Free",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    IsArchived = false,
                    ModulesEnabled = "CRM",
                    UserCount = 3
                },
                new CompanyListItemViewModel
                {
                    CompanyId = Guid.NewGuid().ToString(),
                    Name = "Legacy Systems Corp",
                    AdminEmail = "admin@legacysystems.com",
                    BillingPlan = "Professional",
                    CreatedAt = DateTime.UtcNow.AddYears(-3),
                    IsArchived = true,
                    ModulesEnabled = "CRM, Inventory",
                    UserCount = 0
                },
                new CompanyListItemViewModel
                {
                    CompanyId = Guid.NewGuid().ToString(),
                    Name = "Innovation Labs",
                    AdminEmail = "team@innovationlabs.io",
                    BillingPlan = "Professional",
                    CreatedAt = DateTime.UtcNow.AddMonths(-9),
                    IsArchived = false,
                    ModulesEnabled = "CRM, Sales, HR",
                    UserCount = 25
                },
                new CompanyListItemViewModel
                {
                    CompanyId = Guid.NewGuid().ToString(),
                    Name = "Retail Partners LLC",
                    AdminEmail = "ops@retailpartners.com",
                    BillingPlan = "Enterprise",
                    CreatedAt = DateTime.UtcNow.AddMonths(-15),
                    IsArchived = false,
                    ModulesEnabled = "Inventory, Sales, Finance",
                    UserCount = 67
                }
            };

            // Apply filters
            var filtered = allCompanies.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var query = searchQuery.ToLower();
                filtered = filtered.Where(c =>
                    c.Name.ToLower().Contains(query) ||
                    c.AdminEmail.ToLower().Contains(query) ||
                    c.CompanyId.ToLower().Contains(query));
            }

            if (!string.IsNullOrWhiteSpace(statusFilter) && statusFilter != "All")
            {
                var isArchived = statusFilter == "Archived";
                filtered = filtered.Where(c => c.IsArchived == isArchived);
            }

            if (!string.IsNullOrWhiteSpace(billingPlanFilter) && billingPlanFilter != "All")
            {
                filtered = filtered.Where(c => c.BillingPlan.Equals(billingPlanFilter, StringComparison.OrdinalIgnoreCase));
            }

            var filteredList = filtered.ToList();

            return new ViewCompaniesViewModel
            {
                Companies = filteredList,
                SearchQuery = searchQuery,
                StatusFilter = statusFilter ?? "Active",
                BillingPlanFilter = billingPlanFilter,
                TotalCompanies = filteredList.Count
            };
        }

        // ------------------------------------------------------------
        // GET: Edit company
        // ------------------------------------------------------------
        /// <summary>
        /// Displays the edit form for the specified company.
        /// </summary>
        /// <param name="id">The unique identifier of the company to edit.</param>
        /// <returns>A view that displays the edit form with the company's current data.</returns>
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> EditCompany(string id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ProvisioningApi");
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                var response = await client.GetAsync($"/api/admin/companies/{id}", cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var company = await response.Content.ReadFromJsonAsync<CompanyListItemViewModel>();
                    if (company != null)
                    {
                        var vm = new EditCompanyViewModel
                        {
                            CompanyId = company.CompanyId,
                            Name = company.Name,
                            AdminEmail = company.AdminEmail,
                            BillingPlan = company.BillingPlan,
                            IsArchived = company.IsArchived,
                            ModulesEnabled = company.ModulesEnabled,
                            CreatedAt = company.CreatedAt,
                            GroupedModules = ModuleCatalog.AllModules
                                .GroupBy(m => m.Group)
                                .Select(g => new ModuleGroupViewModel
                                {
                                    GroupName = g.Key,
                                    Modules = g.Select(SelectableModuleDefinition.FromModuleDefinition).ToList()
                                })
                                .ToList(),
                            SelectedModules = company.ModuleList
                        };
                        return View(vm);
                    }
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
            {
                // API unavailable - fall through to mock data
            }

            // Mock fallback
            var mockVm = GetMockEditCompanyViewModel(id);
            if (mockVm == null)
            {
                TempData["Error"] = "Company not found.";
                return RedirectToAction("ViewCompanies");
            }

            return View(mockVm);
        }

        // ------------------------------------------------------------
        // POST: Edit company
        // ------------------------------------------------------------
        /// <summary>
        /// Updates an existing company's details.
        /// </summary>
        /// <param name="vm">The view model containing updated company data.</param>
        /// <returns>Redirects to ViewCompanies on success, or redisplays the form with errors.</returns>
        [HttpPost("edit/{id}")]
        public async Task<IActionResult> EditCompany(EditCompanyViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate modules
                vm.GroupedModules = ModuleCatalog.AllModules
                    .GroupBy(m => m.Group)
                    .Select(g => new ModuleGroupViewModel
                    {
                        GroupName = g.Key,
                        Modules = g.Select(SelectableModuleDefinition.FromModuleDefinition).ToList()
                    })
                    .ToList();
                return View(vm);
            }

            try
            {
                var updateRequest = new
                {
                    companyId = vm.CompanyId,
                    name = vm.Name,
                    adminEmail = vm.AdminEmail,
                    billingPlan = vm.BillingPlan,
                    isArchived = vm.IsArchived,
                    modulesEnabled = string.Join(",", vm.SelectedModules),
                    submodulesEnabled = string.Join(",", vm.SelectedSubmodules),
                    notes = vm.Notes
                };

                var client = _httpClientFactory.CreateClient("ProvisioningApi");
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                var response = await client.PutAsJsonAsync($"/api/admin/companies/{vm.CompanyId}", updateRequest, cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<UpdateCompanyResult>();
                    if (result is not null && result.Success)
                    {
                        TempData["SmartCommit"] = $"Updated company: {vm.Name}";
                        return RedirectToAction("ViewCompanies");
                    }

                    ModelState.AddModelError("", result?.Error ?? "Update failed.");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Update failed: {error}");
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
            {
                // API unavailable - mock success
                TempData["SmartCommit"] = $"Updated company (mock): {vm.Name}";
                TempData["MockMode"] = true;
                return RedirectToAction("ViewCompanies");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }

            // Redisplay form with errors
            vm.GroupedModules = ModuleCatalog.AllModules
                .GroupBy(m => m.Group)
                .Select(g => new ModuleGroupViewModel
                {
                    GroupName = g.Key,
                    Modules = g.Select(SelectableModuleDefinition.FromModuleDefinition).ToList()
                })
                .ToList();
            return View(vm);
        }

        // ------------------------------------------------------------
        // Helper: Get mock edit company view model
        // ------------------------------------------------------------
        private EditCompanyViewModel? GetMockEditCompanyViewModel(string id)
        {
            // Find the company in mock data
            var mockCompanies = GetMockCompaniesViewModel(null, null, null).Companies;
            var company = mockCompanies.FirstOrDefault(c => c.CompanyId == id);

            if (company == null)
                return null;

            return new EditCompanyViewModel
            {
                CompanyId = company.CompanyId,
                Name = company.Name,
                AdminEmail = company.AdminEmail,
                BillingPlan = company.BillingPlan,
                IsArchived = company.IsArchived,
                ModulesEnabled = company.ModulesEnabled,
                CreatedAt = company.CreatedAt,
                GroupedModules = ModuleCatalog.AllModules
                    .GroupBy(m => m.Group)
                    .Select(g => new ModuleGroupViewModel
                    {
                        GroupName = g.Key,
                        Modules = g.Select(SelectableModuleDefinition.FromModuleDefinition).ToList()
                    })
                    .ToList(),
                SelectedModules = company.ModuleList
            };
        }

        // ------------------------------------------------------------
        // POST: Archive/Restore company
        // ------------------------------------------------------------
        /// <summary>
        /// Archives or restores a company based on its current status.
        /// </summary>
        /// <param name="id">The unique identifier of the company.</param>
        /// <param name="returnUrl">Optional URL to redirect after success.</param>
        /// <returns>Redirects to the return URL or ViewCompanies on success.</returns>
        [HttpPost("archive/{id}")]
        public async Task<IActionResult> ArchiveCompany(string id, string? returnUrl)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ProvisioningApi");
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                // First get the company to check current archive status
                var getResponse = await client.GetAsync($"/api/admin/companies/{id}", cts.Token);
                bool isCurrentlyArchived = false;

                if (getResponse.IsSuccessStatusCode)
                {
                    var company = await getResponse.Content.ReadFromJsonAsync<CompanyListItemViewModel>();
                    isCurrentlyArchived = company?.IsArchived ?? false;
                }

                // Toggle the archive status
                var toggleRequest = new { companyId = id, isArchived = !isCurrentlyArchived };
                var response = await client.PostAsJsonAsync($"/api/admin/companies/{id}/archive", toggleRequest, cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<UpdateCompanyResult>();
                    if (result is not null && result.Success)
                    {
                        TempData["SmartCommit"] = isCurrentlyArchived 
                            ? "Company restored successfully." 
                            : "Company archived successfully.";

                        return string.IsNullOrEmpty(returnUrl) 
                            ? RedirectToAction("ViewCompanies") 
                            : Redirect(returnUrl);
                    }

                    TempData["Error"] = result?.Error ?? "Archive operation failed.";
                }
                else
                {
                    TempData["Error"] = "Archive operation failed.";
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
            {
                // API unavailable - mock success
                TempData["SmartCommit"] = "Company archive status updated (mock).";
                TempData["MockMode"] = true;

                return string.IsNullOrEmpty(returnUrl) 
                    ? RedirectToAction("ViewCompanies") 
                    : Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("ViewCompanies");
        }

        // ------------------------------------------------------------
        // GET: Archive company confirmation
        // ------------------------------------------------------------
        /// <summary>
        /// Displays the archive confirmation view for the specified company.
        /// </summary>
        /// <param name="id">The unique identifier of the company to be archived.</param>
        /// <returns>A view that allows the user to confirm archiving the specified company.</returns>
        [HttpGet("archive/{id}")]
        public IActionResult ArchiveCompanyConfirmation(string id) => View("ArchiveCompany", id);

        // ------------------------------------------------------------
        // GET: Merge companies
        // ------------------------------------------------------------
        /// <summary>
        /// Displays the merge companies form.
        /// </summary>
        /// <returns>A view with the merge form and available companies.</returns>
        [HttpGet("merge")]
        public async Task<IActionResult> MergeCompanies()
        {
            var vm = new MergeCompaniesViewModel();

            try
            {
                var client = _httpClientFactory.CreateClient("ProvisioningApi");
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                var response = await client.GetAsync("/api/admin/companies", cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var companies = await response.Content.ReadFromJsonAsync<List<CompanyListItemViewModel>>();
                    if (companies != null)
                    {
                        // Only show active companies for merging
                        vm.AvailableCompanies = companies.Where(c => !c.IsArchived).ToList();
                        return View(vm);
                    }
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
            {
                // API unavailable - fall through to mock data
            }

            // Mock fallback
            vm.AvailableCompanies = GetMockCompaniesViewModel(null, "Active", null).Companies;
            return View(vm);
        }

        // ------------------------------------------------------------
        // POST: Merge companies
        // ------------------------------------------------------------
        /// <summary>
        /// Processes the company merge request.
        /// </summary>
        /// <param name="vm">The merge request details.</param>
        /// <returns>Redirects to ViewCompanies on success, or redisplays the form with errors.</returns>
        [HttpPost("merge")]
        public async Task<IActionResult> MergeCompanies(MergeCompaniesViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate companies
                try
                {
                    var client = _httpClientFactory.CreateClient("ProvisioningApi");
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                    var response = await client.GetAsync("/api/admin/companies", cts.Token);

                    if (response.IsSuccessStatusCode)
                    {
                        var companies = await response.Content.ReadFromJsonAsync<List<CompanyListItemViewModel>>();
                        if (companies != null)
                        {
                            vm.AvailableCompanies = companies.Where(c => !c.IsArchived).ToList();
                        }
                    }
                }
                catch
                {
                    vm.AvailableCompanies = GetMockCompaniesViewModel(null, "Active", null).Companies;
                }

                return View(vm);
            }

            if (vm.SourceCompanyId == vm.TargetCompanyId)
            {
                ModelState.AddModelError("", "Source and target companies must be different.");
                vm.AvailableCompanies = GetMockCompaniesViewModel(null, "Active", null).Companies;
                return View(vm);
            }

            try
            {
                var mergeRequest = new
                {
                    sourceCompanyId = vm.SourceCompanyId,
                    targetCompanyId = vm.TargetCompanyId,
                    transferUsers = vm.TransferUsers,
                    transferData = vm.TransferData,
                    archiveSource = vm.ArchiveSource,
                    notes = vm.Notes
                };

                var client = _httpClientFactory.CreateClient("ProvisioningApi");
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)); // Longer timeout for merge
                var response = await client.PostAsJsonAsync("/api/admin/companies/merge", mergeRequest, cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<UpdateCompanyResult>();
                    if (result is not null && result.Success)
                    {
                        TempData["SmartCommit"] = "Companies merged successfully.";
                        return RedirectToAction("ViewCompanies");
                    }

                    ModelState.AddModelError("", result?.Error ?? "Merge failed.");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Merge failed: {error}");
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
            {
                // API unavailable - mock success
                TempData["SmartCommit"] = "Companies merged successfully (mock).";
                TempData["MockMode"] = true;
                return RedirectToAction("ViewCompanies");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }

            // Redisplay form with errors
            vm.AvailableCompanies = GetMockCompaniesViewModel(null, "Active", null).Companies;
            return View(vm);
        }

        // ------------------------------------------------------------
        // GET: Audit log (calls API)
        // ------------------------------------------------------------
        //[HttpGet("audit/{id:guid}")]
        // ------------------------------------------------------------
        // GET: Audit log
        // ------------------------------------------------------------
        /// <summary>
        /// Displays the audit log for a specific company.
        /// </summary>
        /// <param name="id">The company identifier.</param>
        /// <param name="eventTypeFilter">Optional filter by event type.</param>
        /// <param name="startDate">Optional start date filter.</param>
        /// <param name="endDate">Optional end date filter.</param>
        /// <returns>A view displaying the audit log entries.</returns>
        [HttpGet("audit/{id}")]
        public async Task<IActionResult> AuditLog(string id, string? eventTypeFilter, DateTime? startDate, DateTime? endDate)
        {
            var vm = new AuditLogViewModel
            {
                CompanyId = id,
                EventTypeFilter = eventTypeFilter,
                StartDate = startDate,
                EndDate = endDate
            };

            try
            {
                var client = _httpClientFactory.CreateClient("ProvisioningApi");
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                // Build query string
                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(eventTypeFilter))
                    queryParams.Add($"eventType={Uri.EscapeDataString(eventTypeFilter)}");
                if (startDate.HasValue)
                    queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                if (endDate.HasValue)
                    queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

                var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
                var response = await client.GetAsync($"/api/admin/companies/{id}/audit{queryString}", cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuditLogApiResult>();
                    if (result != null)
                    {
                        vm.CompanyName = result.CompanyName;
                        vm.AuditEntries = result.Entries;
                        return View(vm);
                    }
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
            {
                // API unavailable - fall through to mock data
            }

            // Mock fallback
            vm = GetMockAuditLogViewModel(id, eventTypeFilter, startDate, endDate);
            return View(vm);
        }

        // ------------------------------------------------------------
        // Helper: Get mock audit log view model
        // ------------------------------------------------------------
        private AuditLogViewModel GetMockAuditLogViewModel(string id, string? eventTypeFilter, DateTime? startDate, DateTime? endDate)
        {
            // Find company name
            var mockCompanies = GetMockCompaniesViewModel(null, null, null).Companies;
            var company = mockCompanies.FirstOrDefault(c => c.CompanyId == id);

            var allEntries = new List<AuditEntryViewModel>
            {
                new() { Id = "1", Timestamp = DateTime.Now.AddDays(-30), EventType = "Created", PerformedBy = "admin@businessasusual.com", Description = "Company created" },
                new() { Id = "2", Timestamp = DateTime.Now.AddDays(-25), EventType = "Updated", PerformedBy = "admin@businessasusual.com", Description = "Updated billing plan to Professional" },
                new() { Id = "3", Timestamp = DateTime.Now.AddDays(-20), EventType = "Updated", PerformedBy = "admin@businessasusual.com", Description = "Added Inventory module" },
                new() { Id = "4", Timestamp = DateTime.Now.AddDays(-15), EventType = "Updated", PerformedBy = "admin@businessasusual.com", Description = "Updated company name" },
                new() { Id = "5", Timestamp = DateTime.Now.AddDays(-10), EventType = "Updated", PerformedBy = "admin@businessasusual.com", Description = "Added HR module" },
                new() { Id = "6", Timestamp = DateTime.Now.AddDays(-5), EventType = "Updated", PerformedBy = "admin@businessasusual.com", Description = "Modified admin email" },
                new() { Id = "7", Timestamp = DateTime.Now.AddDays(-2), EventType = "Updated", PerformedBy = "admin@businessasusual.com", Description = "Updated module configuration" }
            };

            // Apply filters
            var filteredEntries = allEntries.AsEnumerable();

            if (!string.IsNullOrEmpty(eventTypeFilter))
                filteredEntries = filteredEntries.Where(e => e.EventType.Equals(eventTypeFilter, StringComparison.OrdinalIgnoreCase));

            if (startDate.HasValue)
                filteredEntries = filteredEntries.Where(e => e.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                filteredEntries = filteredEntries.Where(e => e.Timestamp <= endDate.Value.AddDays(1).AddSeconds(-1));

            return new AuditLogViewModel
            {
                CompanyId = id,
                CompanyName = company?.Name ?? "Unknown Company",
                AuditEntries = filteredEntries.OrderByDescending(e => e.Timestamp).ToList(),
                EventTypeFilter = eventTypeFilter,
                StartDate = startDate,
                EndDate = endDate
            };
        }

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

        private void HydrateModules(ProvisionCompanyViewModel vm)
        {
            vm.GroupedModules = ModuleCatalog.AllModules
                .GroupBy(m => m.Group)
                .Select(g => new ModuleGroupViewModel
                {
                    GroupName = g.Key,
                    Modules = g.Select(SelectableModuleDefinition.FromModuleDefinition).ToList()
                })
                .ToList();
        }

        private void ApplySelections(ProvisionCompanyViewModel vm)
        {
            var selectedModules = (vm.Company.ModulesEnabled ?? "")
                .Split(",", StringSplitOptions.RemoveEmptyEntries);

            var selectedSubmodules = (vm.Company.SubmodulesEnabled ?? "")
                .Split(",", StringSplitOptions.RemoveEmptyEntries);

            foreach (var group in vm.GroupedModules)
            {
                foreach (var module in group.Modules)
                {
                    module.IsSelected = selectedModules.Contains(module.Key);

                    foreach (var sub in module.Submodules)
                    {
                        sub.IsSelected = selectedSubmodules.Contains(sub.Key);
                    }
                }
            }
        }

        // ------------------------------------------------------------
        // Module Usage
        // ------------------------------------------------------------

        /// <summary>
        /// Displays the module usage dashboard for companies.
        /// </summary>
        [HttpGet("module-usage")]
        public IActionResult ModuleUsage()
        {
            return View();
        }

        /// <summary>
        /// API endpoint to get module usage data for a specific company.
        /// </summary>
        [HttpGet("/api/admin/company/{companyId}/module-usage")]
        public async Task<IActionResult> GetModuleUsage(int companyId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AdminApi");
                var response = await client.GetAsync($"/api/admin/company/{companyId}/module-usage");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<object>();
                    return Json(data);
                }
            }
            catch (Exception)
            {
                // Fall through to mock
            }

            // Mock fallback data
            return Json(GetMockModuleUsageData(companyId));
        }

        /// <summary>
        /// API endpoint to get list of companies for the picker.
        /// </summary>
        [HttpGet("/api/admin/companies/list")]
        public async Task<IActionResult> GetCompaniesList()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AdminApi");
                var response = await client.GetAsync("/api/admin/companies/list");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<object>();
                    return Json(data);
                }
            }
            catch (Exception)
            {
                // Fall through to mock
            }

            // Mock fallback
            return Json(new[]
            {
                new { id = 1, name = "Acme Corporation", status = "Active" },
                new { id = 2, name = "TechStart Inc", status = "Active" },
                new { id = 3, name = "Global Traders LLC", status = "Active" },
                new { id = 4, name = "Innovate Solutions", status = "Active" },
                new { id = 5, name = "Enterprise Systems", status = "Trial" }
            });
        }

        private object GetMockModuleUsageData(int companyId)
        {
            return new
            {
                companyId,
                companyName = companyId switch
                {
                    1 => "Acme Corporation",
                    2 => "TechStart Inc",
                    3 => "Global Traders LLC",
                    4 => "Innovate Solutions",
                    5 => "Enterprise Systems",
                    _ => "Unknown Company"
                },
                billingPeriod = "January 2026",
                totalCost = 2847.50,
                totalUsageMinutes = 156780,
                modules = new[]
                {
                    new
                    {
                        name = "CRM",
                        icon = "fa-users",
                        color = "#3b82f6",
                        usageMinutes = 45600,
                        cost = 684.00,
                        costPerHour = 0.90,
                        submodules = new[]
                        {
                            new { name = "Contact Management", usageMinutes = 18240, cost = 273.60 },
                            new { name = "Lead Tracking", usageMinutes = 15200, cost = 228.00 },
                            new { name = "Sales Pipeline", usageMinutes = 12160, cost = 182.40 }
                        }
                    },
                    new
                    {
                        name = "Finance",
                        icon = "fa-dollar-sign",
                        color = "#10b981",
                        usageMinutes = 38400,
                        cost = 768.00,
                        costPerHour = 1.20,
                        submodules = new[]
                        {
                            new { name = "Invoicing", usageMinutes = 19200, cost = 384.00 },
                            new { name = "Expense Tracking", usageMinutes = 12800, cost = 256.00 },
                            new { name = "Financial Reports", usageMinutes = 6400, cost = 128.00 }
                        }
                    },
                    new
                    {
                        name = "Inventory",
                        icon = "fa-boxes-stacked",
                        color = "#8b5cf6",
                        usageMinutes = 28800,
                        cost = 432.00,
                        costPerHour = 0.90,
                        submodules = new[]
                        {
                            new { name = "Stock Management", usageMinutes = 14400, cost = 216.00 },
                            new { name = "Warehouse Ops", usageMinutes = 9600, cost = 144.00 },
                            new { name = "Asset Tracking", usageMinutes = 4800, cost = 72.00 }
                        }
                    },
                    new
                    {
                        name = "Sales",
                        icon = "fa-chart-line",
                        color = "#f59e0b",
                        usageMinutes = 24960,
                        cost = 499.20,
                        costPerHour = 1.20,
                        submodules = new[]
                        {
                            new { name = "Order Processing", usageMinutes = 12480, cost = 249.60 },
                            new { name = "Quote Generation", usageMinutes = 8320, cost = 166.40 },
                            new { name = "Sales Analytics", usageMinutes = 4160, cost = 83.20 }
                        }
                    },
                    new
                    {
                        name = "Reporting",
                        icon = "fa-file-chart-column",
                        color = "#06b6d4",
                        usageMinutes = 19020,
                        cost = 285.30,
                        costPerHour = 0.90,
                        submodules = new[]
                        {
                            new { name = "Dashboard Builder", usageMinutes = 9510, cost = 142.65 },
                            new { name = "Custom Reports", usageMinutes = 6340, cost = 95.10 },
                            new { name = "Data Export", usageMinutes = 3170, cost = 47.55 }
                        }
                    }
                }
            };
        }
    }

    /// <summary>
    /// Represents the result of a company update operation.
    /// </summary>
    public class UpdateCompanyResult
    {
        /// <summary>
        /// Gets or sets whether the update was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets any error message if the operation failed.
        /// </summary>
        public string? Error { get; set; }
    }

    /// <summary>
    /// Represents the API response for audit log requests.
    /// </summary>
    public class AuditLogApiResult
    {
        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of audit entries.
        /// </summary>
        public List<AuditEntryViewModel> Entries { get; set; } = new();
    }
}
