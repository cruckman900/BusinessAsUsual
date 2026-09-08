using BusinessAsUsual.Application.Contracts;
using BusinessAsUsual.Application.Services.Provisioning;
using BusinessAsUsual.Infrastructure.Middleware;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BusinessAsUsual.API.Controllers
{
    /// <summary>
    /// Represents an API controller that handles company provisioning requests.
    /// </summary>
    /// <remarks>This controller exposes endpoints for initiating company provisioning operations via HTTP. It
    /// is intended to be used by clients that need to provision new companies or tenants in the system. All routes are
    /// prefixed with 'api/provisioning'.
    /// Exempt from mandatory tenant resolution: provisioning/creating a new tenant and listing
    /// existing tenants (for the login page's tenant switcher) necessarily happen before any
    /// tenant context can be known.</remarks>
    [ApiController]
    [Route("api/provisioning")]
    [AllowAnonymousTenant]
    public class ProvisioningApiController : ControllerBase
    {
        private readonly IProvisioningService _provisioner;

        /// <summary>
        /// Initializes a new instance of the ProvisioningApiController class with the specified provisioning service.
        /// </summary>
        /// <param name="provisioner">The provisioning service used to handle provisioning operations. Cannot be null.</param>
        public ProvisioningApiController(IProvisioningService provisioner)
        {
            _provisioner = provisioner;
        }

        /// <summary>
        /// Provisions a new company tenant based on the specified provisioning request.
        /// </summary>
        /// <param name="request">The details required to provision the company, including company information and configuration settings.
        /// Cannot be null.</param>
        /// <returns>An <see cref="IActionResult"/> containing the result of the provisioning operation. Returns a 200 OK
        /// response with company and tenant details if successful; otherwise, returns a 400 Bad Request with error
        /// information.</returns>
        [HttpPost("provision-company")]
        public async Task<IActionResult> ProvisionCompany([FromBody] ProvisioningRequest request)
        {
            var result = await _provisioner.ProvisionTenantAsync(request);

            if (!result.Success)
                return BadRequest(new { success = false, error = result.Error });

            return Ok(new
            {
                success = true,
                message = "Provisioning successful",
                companyId = result.CompanyId,
                tenantDbName = result.TenantDbName
            });
        }

        /// <summary>
        /// Retrieves all provisioned companies, for use by tenant selection UIs
        /// (e.g. the Web shell's login page tenant switcher).
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing the list of companies with their Id, Name, and DbName.</returns>
        [HttpGet("companies")]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _provisioner.GetAllCompaniesAsync();

            return Ok(companies.Select(c => new
            {
                companyId = c.Id,
                name = c.Name,
                dbName = c.DbName
            }));
        }
    }
}
