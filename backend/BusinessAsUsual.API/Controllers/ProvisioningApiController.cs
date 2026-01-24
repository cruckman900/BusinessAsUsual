using BusinessAsUsual.Admin.Areas.Admin.Models;
using BusinessAsUsual.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Areas.Admin.Controllers
{
    /// <summary>
    /// Represents an API controller that handles company provisioning requests.
    /// </summary>
    /// <remarks>This controller exposes endpoints for initiating company provisioning operations via HTTP. It
    /// is intended to be used by clients that need to provision new companies or tenants in the system. All routes are
    /// prefixed with 'api/provisioning'.</remarks>
    [ApiController]
    [Route("api/provisioning")]
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
    }
}
