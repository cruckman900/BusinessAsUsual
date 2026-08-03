using BusinessAsUsual.Admin.Areas.Admin.Models;
using BusinessAsUsual.Admin.Attributes;
using BusinessAsUsual.Admin.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BusinessAsUsual.Admin.Areas.Admin.Controllers
{
    /// <summary>
    /// Provides administrative actions for managing admin portal users, including adding new users,
    /// editing user details, and managing user roles within the admin area.
    /// </summary>
    /// <remarks>
    /// This controller is intended for use within the administrative section of the application and
    /// exposes endpoints for admin user lifecycle management. Most actions require appropriate administrative permissions.
    /// API calls to external services are performed using an HTTP client factory with fallback to mock data.
    /// </remarks>
    [Area("Admin")]
    [Route("admin/user")]
    [AdminAuth]
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHubContext<SmartCommitHub> _hubContext;

        /// <summary>
        /// Initializes a new instance of the UserController class with the specified HTTP client factory and SignalR
        /// hub context.
        /// </summary>
        /// <param name="httpClientFactory">The factory used to create HttpClient instances for making HTTP requests.</param>
        /// <param name="hubContext">The SignalR hub context used to communicate with connected clients via the SmartCommitHub.</param>
        public UserController(
            IHttpClientFactory httpClientFactory,
            IHubContext<SmartCommitHub> hubContext)
        {
            _httpClientFactory = httpClientFactory;
            _hubContext = hubContext;
        }

        // ------------------------------------------------------------
        // GET: Add User form
        // ------------------------------------------------------------
        /// <summary>
        /// Handles HTTP GET requests for the add user form. Prepares and returns a view that allows admins
        /// to create a new admin portal user.
        /// </summary>
        /// <remarks>
        /// The returned view model includes an empty user instance and a list of available admin roles.
        /// This endpoint is typically used to display the initial user creation form before any user data has been submitted.
        /// </remarks>
        /// <returns>A view result that renders the add user form with available role options.</returns>
        [HttpGet("add")]
        public IActionResult AddUser()
        {
            var vm = new AddUserViewModel
            {
                // Populate available roles (stubbed for now - will be replaced with API call later)
                AvailableRoles = GetStubRoles(),
                IsActive = true // Default to active
            };

            return View(vm);
        }

        // ------------------------------------------------------------
        // POST: Add User (calls API with fallback to mock)
        // ------------------------------------------------------------
        /// <summary>
        /// Handles a POST request to create a new admin portal user by submitting the provided user details to the
        /// admin user API.
        /// </summary>
        /// <remarks>
        /// If the model state is invalid or the API call fails, the method redisplays the form with appropriate error messages.
        /// On successful user creation, the user is redirected to a confirmation page. If the API is unavailable,
        /// falls back to mock/in-memory success for development purposes.
        /// </remarks>
        /// <param name="vm">A view model containing the user information to be created. Must not be null and must contain valid data.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> that renders the add user form with validation errors if the input is invalid or creation fails;
        /// otherwise, a redirect to the add user success page.
        /// </returns>
        [HttpPost("add")]
        public async Task<IActionResult> AddUser(AddUserViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate roles on validation failure
                vm.AvailableRoles = GetStubRoles();
                return View(vm);
            }

            try
            {
                // Prepare the request payload
                var userRequest = new
                {
                    userName = vm.UserName,
                    email = vm.Email,
                    password = vm.Password,
                    firstName = vm.FirstName,
                    lastName = vm.LastName,
                    isActive = vm.IsActive,
                    roles = vm.SelectedRoles,
                    notes = vm.Notes
                };

                // Try to call the API with a short timeout
                var client = _httpClientFactory.CreateClient("AdminApi");
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                var response = await client.PostAsJsonAsync("/api/admin/users", userRequest, cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<UserCreationResult>();

                    if (result is not null && result.Success)
                    {
                        TempData["SmartCommit"] = $"Added admin user: {vm.UserName}";
                        TempData["NewUserId"] = result.UserId;
                        TempData["NewUserName"] = vm.UserName;
                        TempData["NewUserEmail"] = vm.Email;
                        return RedirectToAction("AddUserSuccess");
                    }

                    ModelState.AddModelError("", result?.Error ?? "User creation failed.");
                    vm.AvailableRoles = GetStubRoles();
                    return View(vm);
                }

                // API returned non-success - try to read error message
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"User creation failed: {error}");
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
            {
                // API unavailable or timeout - fallback to mock success for development
                TempData["SmartCommit"] = $"Added admin user (mock): {vm.UserName}";
                TempData["NewUserId"] = Guid.NewGuid().ToString();
                TempData["NewUserName"] = vm.UserName;
                TempData["NewUserEmail"] = vm.Email;
                TempData["MockMode"] = true;
                return RedirectToAction("AddUserSuccess");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }

            // If we got here, something went wrong - redisplay form
            vm.AvailableRoles = GetStubRoles();
            return View(vm);
        }

        // ------------------------------------------------------------
        // GET: Add User Success
        // ------------------------------------------------------------
        /// <summary>
        /// Handles GET requests for the add user success page and returns the corresponding view.
        /// </summary>
        /// <returns>A view result that renders the add user success page.</returns>
        [HttpGet("add/success")]
        public IActionResult AddUserSuccess()
        {
            ViewBag.CommitTag = TempData["SmartCommit"];
            ViewBag.UserId = TempData["NewUserId"];
            ViewBag.UserName = TempData["NewUserName"];
            ViewBag.UserEmail = TempData["NewUserEmail"];
            ViewBag.MockMode = TempData["MockMode"] as bool? ?? false;
            return View();
        }

        // ------------------------------------------------------------
        // Helper: Get stub roles (temporary - will be replaced with API call)
        // ------------------------------------------------------------
        /// <summary>
        /// Returns a stubbed list of admin roles for development purposes.
        /// This will be replaced with an API call to fetch actual roles later.
        /// </summary>
        /// <returns>A list of admin role options.</returns>
        private static List<AdminRoleOption> GetStubRoles()
        {
            return new List<AdminRoleOption>
            {
                new AdminRoleOption
                {
                    RoleId = "super-admin",
                    RoleName = "Super Admin",
                    Description = "Full access to all admin portal features, including user management and system configuration.",
                    IsSelected = false
                },
                new AdminRoleOption
                {
                    RoleId = "company-manager",
                    RoleName = "Company Manager",
                    Description = "Can provision, edit, merge, and archive companies. Can view audit logs.",
                    IsSelected = false
                },
                new AdminRoleOption
                {
                    RoleId = "user-manager",
                    RoleName = "User Manager",
                    Description = "Can add, edit, and deactivate admin portal users and assign roles.",
                    IsSelected = false
                },
                new AdminRoleOption
                {
                    RoleId = "monitor",
                    RoleName = "Monitor",
                    Description = "Can view health monitoring, metrics, and logs. Read-only access.",
                    IsSelected = false
                },
                new AdminRoleOption
                {
                    RoleId = "viewer",
                    RoleName = "Viewer",
                    Description = "Read-only access to most admin portal features. Cannot make changes.",
                    IsSelected = false
                }
            };
        }

        // ------------------------------------------------------------
        // GET: Manage Users
        // ------------------------------------------------------------
        /// <summary>
        /// Handles HTTP GET requests for the manage users page. Displays a list of all admin portal users
        /// with search and filter capabilities.
        /// </summary>
        /// <param name="searchQuery">Optional search query to filter users by name or email.</param>
        /// <param name="roleFilter">Optional role filter.</param>
        /// <param name="statusFilter">Optional status filter (Active, Inactive, All).</param>
        /// <param name="page">Current page number for pagination.</param>
        /// <returns>A view result that renders the manage users page with the filtered user list.</returns>
        [HttpGet("manage")]
        public async Task<IActionResult> ManageUsers(
            string? searchQuery = null,
            string? roleFilter = null,
            string? statusFilter = null,
            int page = 1)
        {
            try
            {
                // Try to call the API with a short timeout
                var client = _httpClientFactory.CreateClient("AdminApi");
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(searchQuery))
                    queryParams.Add($"search={Uri.EscapeDataString(searchQuery)}");
                if (!string.IsNullOrEmpty(roleFilter))
                    queryParams.Add($"role={Uri.EscapeDataString(roleFilter)}");
                if (!string.IsNullOrEmpty(statusFilter))
                    queryParams.Add($"status={Uri.EscapeDataString(statusFilter)}");
                queryParams.Add($"page={page}");

                var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";

                // Use a cancellation token with short timeout for faster fallback
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                var response = await client.GetAsync($"/api/admin/users{queryString}", cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var apiVm = await response.Content.ReadFromJsonAsync<ManageUsersViewModel>();
                    if (apiVm != null)
                    {
                        return View(apiVm);
                    }
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
            {
                // API unavailable or timeout - fall through to mock data
                // Log could be added here if needed for diagnostics
            }

            // Fallback to mock data
            var vm = GetMockUsersViewModel(searchQuery, roleFilter, statusFilter, page);
            return View(vm);
        }

        // ------------------------------------------------------------
        // Helper: Get mock users (temporary - will be replaced with API call)
        // ------------------------------------------------------------
        /// <summary>
        /// Returns a stubbed list of admin users for development purposes.
        /// This will be replaced with an API call to fetch actual users later.
        /// </summary>
        private ManageUsersViewModel GetMockUsersViewModel(
            string? searchQuery,
            string? roleFilter,
            string? statusFilter,
            int page)
        {
            // Generate mock users
            var allUsers = new List<UserListItemViewModel>
            {
                new UserListItemViewModel
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = "admin",
                    Email = "admin@businessasusual.com",
                    FirstName = "Admin",
                    LastName = "User",
                    IsActive = true,
                    Roles = "Super Admin",
                    CreatedAt = DateTime.UtcNow.AddMonths(-6),
                    LastLoginAt = DateTime.UtcNow.AddHours(-2)
                },
                new UserListItemViewModel
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = "john.doe",
                    Email = "john.doe@businessasusual.com",
                    FirstName = "John",
                    LastName = "Doe",
                    IsActive = true,
                    Roles = "Company Manager, User Manager",
                    CreatedAt = DateTime.UtcNow.AddMonths(-4),
                    LastLoginAt = DateTime.UtcNow.AddDays(-1)
                },
                new UserListItemViewModel
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = "jane.smith",
                    Email = "jane.smith@businessasusual.com",
                    FirstName = "Jane",
                    LastName = "Smith",
                    IsActive = true,
                    Roles = "Monitor",
                    CreatedAt = DateTime.UtcNow.AddMonths(-3),
                    LastLoginAt = DateTime.UtcNow.AddHours(-5)
                },
                new UserListItemViewModel
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = "bob.johnson",
                    Email = "bob.johnson@businessasusual.com",
                    FirstName = "Bob",
                    LastName = "Johnson",
                    IsActive = false,
                    Roles = "Viewer",
                    CreatedAt = DateTime.UtcNow.AddMonths(-5),
                    LastLoginAt = DateTime.UtcNow.AddDays(-30)
                },
                new UserListItemViewModel
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = "alice.williams",
                    Email = "alice.williams@businessasusual.com",
                    FirstName = "Alice",
                    LastName = "Williams",
                    IsActive = true,
                    Roles = "Company Manager",
                    CreatedAt = DateTime.UtcNow.AddMonths(-2),
                    LastLoginAt = DateTime.UtcNow.AddMinutes(-30)
                },
                new UserListItemViewModel
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = "charlie.brown",
                    Email = "charlie.brown@businessasusual.com",
                    FirstName = "Charlie",
                    LastName = "Brown",
                    IsActive = true,
                    Roles = "User Manager",
                    CreatedAt = DateTime.UtcNow.AddMonths(-1),
                    LastLoginAt = DateTime.UtcNow.AddDays(-3)
                },
                new UserListItemViewModel
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = "david.miller",
                    Email = "david.miller@businessasusual.com",
                    FirstName = "David",
                    LastName = "Miller",
                    IsActive = false,
                    Roles = "Monitor",
                    CreatedAt = DateTime.UtcNow.AddMonths(-7),
                    LastLoginAt = DateTime.UtcNow.AddDays(-60)
                },
                new UserListItemViewModel
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = "emma.davis",
                    Email = "emma.davis@businessasusual.com",
                    FirstName = "Emma",
                    LastName = "Davis",
                    IsActive = true,
                    Roles = "Viewer",
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    LastLoginAt = DateTime.UtcNow.AddHours(-8)
                },
                new UserListItemViewModel
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = "frank.wilson",
                    Email = "frank.wilson@businessasusual.com",
                    FirstName = "Frank",
                    LastName = "Wilson",
                    IsActive = true,
                    Roles = "Monitor, Viewer",
                    CreatedAt = DateTime.UtcNow.AddMonths(-3),
                    LastLoginAt = null
                },
                new UserListItemViewModel
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = "grace.lee",
                    Email = "grace.lee@businessasusual.com",
                    FirstName = "Grace",
                    LastName = "Lee",
                    IsActive = true,
                    Roles = "Company Manager",
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    LastLoginAt = DateTime.UtcNow.AddDays(-2)
                }
            };

            // Apply filters
            var filtered = allUsers.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var query = searchQuery.ToLower();
                filtered = filtered.Where(u =>
                    u.UserName.ToLower().Contains(query) ||
                    u.Email.ToLower().Contains(query) ||
                    u.DisplayName.ToLower().Contains(query));
            }

            if (!string.IsNullOrWhiteSpace(roleFilter) && roleFilter != "All")
            {
                filtered = filtered.Where(u => u.Roles.Contains(roleFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(statusFilter) && statusFilter != "All")
            {
                var isActive = statusFilter == "Active";
                filtered = filtered.Where(u => u.IsActive == isActive);
            }

            var filteredList = filtered.ToList();
            var totalUsers = filteredList.Count;

            // Apply pagination
            var pageSize = 10;
            var pagedUsers = filteredList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new ManageUsersViewModel
            {
                Users = pagedUsers,
                SearchQuery = searchQuery,
                RoleFilter = roleFilter,
                StatusFilter = statusFilter ?? "All",
                CurrentPage = page,
                PageSize = pageSize,
                TotalUsers = totalUsers,
                AvailableRoles = new List<string>
                {
                    "All",
                    "Super Admin",
                    "Company Manager",
                    "User Manager",
                    "Monitor",
                    "Viewer"
                }
            };
        }

        // ------------------------------------------------------------
        // GET: Get User by ID (for editing)
        // ------------------------------------------------------------
        /// <summary>
        /// Retrieves user details by ID for editing.
        /// </summary>
        /// <param name="id">The user ID to retrieve.</param>
        /// <returns>JSON result containing the user data or error.</returns>
        [HttpGet("api/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AdminApi");
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                var response = await client.GetAsync($"/api/admin/users/{id}", cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserListItemViewModel>();
                    return Json(new { success = true, user });
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
            {
                // API unavailable or timeout - use mock data
            }

            // Mock fallback
            var mockUser = GetMockUser(id);
            if (mockUser != null)
            {
                return Json(new { success = true, user = mockUser });
            }

            return Json(new { success = false, error = "User not found." });
        }

        // ------------------------------------------------------------
        // PUT: Update User
        // ------------------------------------------------------------
        /// <summary>
        /// Updates an existing user's details.
        /// </summary>
        /// <param name="id">The user ID to update.</param>
        /// <param name="model">The updated user data.</param>
        /// <returns>JSON result indicating success or failure.</returns>
        [HttpPut("api/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest model)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AdminApi");
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                var response = await client.PutAsJsonAsync($"/api/admin/users/{id}", model, cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<UserUpdateResult>();
                    if (result is not null && result.Success)
                    {
                        TempData["SmartCommit"] = $"Updated user: {model.UserName}";
                        return Json(new { success = true, message = "User updated successfully." });
                    }

                    return Json(new { success = false, error = result?.Error ?? "Update failed." });
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
            {
                // API unavailable or timeout - mock success
                TempData["SmartCommit"] = $"Updated user (mock): {model.UserName}";
                return Json(new { success = true, message = "User updated successfully (mock mode)." });
            }

            return Json(new { success = false, error = "Update failed." });
        }

        // ------------------------------------------------------------
        // POST: Toggle User Status (Activate/Deactivate)
        // ------------------------------------------------------------
        /// <summary>
        /// Toggles a user's active status.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="isActive">The new active status.</param>
        /// <returns>JSON result indicating success or failure.</returns>
        [HttpPost("api/{id}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(string id, [FromBody] ToggleStatusRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AdminApi");
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                var response = await client.PostAsJsonAsync($"/api/admin/users/{id}/toggle-status", request, cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<UserUpdateResult>();
                    if (result is not null && result.Success)
                    {
                        var status = request.IsActive ? "activated" : "deactivated";
                        TempData["SmartCommit"] = $"User {status}";
                        return Json(new { success = true, message = $"User {status} successfully.", isActive = request.IsActive });
                    }

                    return Json(new { success = false, error = result?.Error ?? "Status toggle failed." });
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
            {
                // API unavailable or timeout - mock success
                var status = request.IsActive ? "activated" : "deactivated";
                TempData["SmartCommit"] = $"User {status} (mock)";
                return Json(new { success = true, message = $"User {status} successfully (mock mode).", isActive = request.IsActive });
            }

            return Json(new { success = false, error = "Status toggle failed." });
        }

        // ------------------------------------------------------------
        // DELETE: Delete User
        // ------------------------------------------------------------
        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <param name="id">The user ID to delete.</param>
        /// <returns>JSON result indicating success or failure.</returns>
        [HttpDelete("api/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AdminApi");
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                var response = await client.DeleteAsync($"/api/admin/users/{id}", cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<UserUpdateResult>();
                    if (result is not null && result.Success)
                    {
                        TempData["SmartCommit"] = "User deleted";
                        return Json(new { success = true, message = "User deleted successfully." });
                    }

                    return Json(new { success = false, error = result?.Error ?? "Delete failed." });
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
            {
                // API unavailable or timeout - mock success
                TempData["SmartCommit"] = "User deleted (mock)";
                return Json(new { success = true, message = "User deleted successfully (mock mode)." });
            }

            return Json(new { success = false, error = "Delete failed." });
        }

        // ------------------------------------------------------------
        // Helper: Get mock user by ID
        // ------------------------------------------------------------
        private UserListItemViewModel? GetMockUser(string id)
        {
            // In a real implementation, this would query the database
            // For now, return a mock user
            return new UserListItemViewModel
            {
                UserId = id,
                UserName = "mock.user",
                Email = "mock.user@businessasusual.com",
                FirstName = "Mock",
                LastName = "User",
                IsActive = true,
                Roles = "Viewer",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                LastLoginAt = DateTime.UtcNow.AddHours(-2)
            };
        }
    }

    // ------------------------------------------------------------
    // Response DTOs
    // ------------------------------------------------------------

    /// <summary>
    /// Represents the result of a user creation API call.
    /// </summary>
    public class UserCreationResult
    {
        /// <summary>
        /// Gets or sets whether the user creation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the created user.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Gets or sets any error message if the operation failed.
        /// </summary>
        public string? Error { get; set; }
    }

    /// <summary>
    /// Represents the result of a user update/delete API call.
    /// </summary>
    public class UserUpdateResult
    {
        /// <summary>
        /// Gets or sets whether the operation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets any error message if the operation failed.
        /// </summary>
        public string? Error { get; set; }
    }

    /// <summary>
    /// Request model for updating a user.
    /// </summary>
    public class UpdateUserRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new();
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Request model for toggling user status.
    /// </summary>
    public class ToggleStatusRequest
    {
        public bool IsActive { get; set; }
    }
}
