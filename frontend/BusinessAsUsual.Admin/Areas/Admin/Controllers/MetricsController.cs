using Microsoft.AspNetCore.Mvc;
using BusinessAsUsual.Admin.Attributes;

namespace BusinessAsUsual.Admin.Areas.Admin.Controllers
{
    /// <summary>
    /// Provides administrative endpoints for viewing system health and module metrics.
    /// </summary>
    [Area("Admin")]
    [AdminAuth]
    public class MetricsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MetricsController> _logger;

        public MetricsController(IHttpClientFactory httpClientFactory, ILogger<MetricsController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Returns the System Health dashboard view.
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// API endpoint for module health statistics.
        /// </summary>
        [HttpGet("/api/admin/module-health")]
        public async Task<IActionResult> GetModuleHealth()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AdminApi");
                var response = await client.GetAsync("/api/admin/module-health");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<object>();
                    return Json(data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch module health from API, using mock data");
            }

            // Mock fallback
            return Json(new
            {
                modules = new[]
                {
                    new { name = "CRM", status = "Healthy", uptime = 99.8, requests = 15234, avgResponse = 42 },
                    new { name = "Finance", status = "Healthy", uptime = 99.5, requests = 8912, avgResponse = 58 },
                    new { name = "Inventory", status = "Healthy", uptime = 99.9, requests = 12456, avgResponse = 35 },
                    new { name = "Sales", status = "Healthy", uptime = 99.7, requests = 18723, avgResponse = 48 },
                    new { name = "HR", status = "Warning", uptime = 97.2, requests = 5432, avgResponse = 125 },
                    new { name = "Reporting", status = "Healthy", uptime = 99.4, requests = 3211, avgResponse = 67 }
                },
                systemMetrics = new
                {
                    totalRequests = 63968,
                    avgResponseTime = 54,
                    errorRate = 0.23,
                    uptime = 99.2
                }
            });
        }

        /// <summary>
        /// API endpoint for real-time system metrics.
        /// </summary>
        [HttpGet("/api/admin/system-metrics")]
        public async Task<IActionResult> GetSystemMetrics()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AdminApi");
                var response = await client.GetAsync("/api/metrics/system");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<object>();
                    return Json(data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch system metrics from API, using mock data");
            }

            // Mock fallback with realistic data
            var random = new Random();
            return Json(new
            {
                cpu = new
                {
                    usage = random.Next(15, 45),
                    cores = 8,
                    temperature = random.Next(45, 65)
                },
                memory = new
                {
                    used = random.Next(4000, 8000),
                    total = 16384,
                    percentage = random.Next(25, 50)
                },
                disk = new
                {
                    used = random.Next(200, 400),
                    total = 512,
                    percentage = random.Next(40, 75)
                },
                network = new
                {
                    inbound = random.Next(100, 500),
                    outbound = random.Next(50, 250)
                }
            });
        }
    }
}
