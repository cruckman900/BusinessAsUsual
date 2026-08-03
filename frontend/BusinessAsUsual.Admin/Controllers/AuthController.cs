using BusinessAsUsual.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BusinessAsUsual.Admin.Controllers
{
    /// <summary>
    /// Handles authentication for the admin portal.
    /// </summary>
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IHttpClientFactory httpClientFactory, ILogger<AuthController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates a user via API or mock fallback.
        /// </summary>
        /// <param name="model">The login credentials.</param>
        /// <returns>JSON result indicating success or failure.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new LoginResult
                {
                    Success = false,
                    Error = "Please provide both username and password."
                });
            }

            try
            {
                var client = _httpClientFactory.CreateClient("ProvisioningApi");
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                var loginRequest = new { username = model.Username, password = model.Password };
                var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest, cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResult>();
                    if (result is not null && result.Success)
                    {
                        // Set session
                        HttpContext.Session.SetString("IsAuthenticated", "true");
                        HttpContext.Session.SetString("Username", result.Username ?? model.Username);
                        _logger.LogInformation("User {Username} logged in via API", result.Username);

                        return Json(new LoginResult
                        {
                            Success = true,
                            Username = result.Username ?? model.Username
                        });
                    }
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
            {
                _logger.LogWarning("Auth API unavailable, falling back to mock authentication");
                // Fall through to mock
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication");
            }

            // Mock authentication fallback
            if (model.Username.Equals("admin", StringComparison.OrdinalIgnoreCase) && 
                model.Password == "password")
            {
                HttpContext.Session.SetString("IsAuthenticated", "true");
                HttpContext.Session.SetString("Username", "admin");
                HttpContext.Session.SetString("IsMockAuth", "true");
                _logger.LogInformation("User admin logged in via mock authentication");

                return Json(new LoginResult
                {
                    Success = true,
                    Username = "admin"
                });
            }

            return Json(new LoginResult
            {
                Success = false,
                Error = "Invalid username or password."
            });
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        /// <returns>Redirects to home page.</returns>
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var username = HttpContext.Session.GetString("Username");
            HttpContext.Session.Clear();
            _logger.LogInformation("User {Username} logged out", username);

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Checks if the current user is authenticated.
        /// </summary>
        /// <returns>JSON result with authentication status.</returns>
        [HttpGet("status")]
        public IActionResult Status()
        {
            var isAuthenticated = HttpContext.Session.GetString("IsAuthenticated") == "true";
            var username = HttpContext.Session.GetString("Username");

            return Json(new
            {
                isAuthenticated,
                username
            });
        }
    }
}
