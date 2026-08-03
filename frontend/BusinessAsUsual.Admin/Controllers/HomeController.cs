using BusinessAsUsual.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Controllers
{
    /// <summary>
    /// Represents the controller responsible for handling requests to the application's home page.
    /// </summary>
    /// <remarks>This controller typically serves the default landing page of the application. It inherits
    /// from <see cref="Controller"/>, providing standard MVC controller functionality.</remarks>
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IHttpClientFactory httpClientFactory, ILogger<HomeController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Handles requests for the default page of the controller.
        /// </summary>
        /// <returns>A view that renders the default page.</returns>
        public IActionResult Index()
        {
            // Check authentication status
            ViewBag.IsAuthenticated = HttpContext.Session.GetString("IsAuthenticated") == "true";
            ViewBag.Username = HttpContext.Session.GetString("Username");
            return View();
        }

        /// <summary>
        /// Gets admin dashboard insights (company count, user count, system status).
        /// </summary>
        /// <returns>JSON with insights data.</returns>
        [HttpGet("api/insights")]
        public async Task<IActionResult> GetInsights()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ProvisioningApi");
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                var response = await client.GetAsync("/api/admin/insights", cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var insights = await response.Content.ReadFromJsonAsync<AdminInsightsViewModel>();
                    if (insights != null)
                    {
                        return Json(insights);
                    }
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
            {
                _logger.LogWarning("Insights API unavailable, using mock data");
                // Fall through to mock
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching insights");
            }

            // Mock fallback
            var mockInsights = new AdminInsightsViewModel
            {
                TotalCompanies = 7,
                ActiveCompanies = 5,
                TotalAdminUsers = 12,
                SystemStatus = "Healthy",
                UptimeHours = 168, // 7 days
                IsMockData = true
            };

            return Json(mockInsights);
        }
    }
}
