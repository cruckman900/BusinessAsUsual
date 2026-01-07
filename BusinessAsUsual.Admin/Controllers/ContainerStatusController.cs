using Microsoft.AspNetCore.Mvc;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace BusinessAsUsual.Admin.Controllers
{
    /// <summary>
    /// Provides administrative endpoints for retrieving the status and details of Docker containers.
    /// </summary>
    /// <remarks>This controller is intended for use in administrative scenarios where monitoring or
    /// management of container states is required. All routes are prefixed with 'admin/containers'. The controller
    /// returns container information including names, state, status, and image for all containers, regardless of their
    /// running state.</remarks>
    [ApiController]
    [Route("admin/containers")]
    public class ContainerStatusController : ControllerBase
    {
        /// <summary>
        /// Retrieves a list of all Docker containers, including both running and stopped containers.
        /// </summary>
        /// <remarks>This method returns information for all containers, regardless of their current
        /// state. The result does not include detailed container configuration or logs. The endpoint is intended for
        /// use in monitoring or management scenarios where a high-level overview of containers is required.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing a collection of container summaries. Each summary includes the
        /// container's names, state, status, and image information.</returns>
        [HttpGet]
        public IActionResult GetContainers()
        {
            var client = new DockerClientConfiguration().CreateClient();

            var containers = client.Containers.ListContainersAsync(
                new ContainersListParameters { All = true }
            ).Result;

            return Ok(containers.Select(c => new
            {
                c.Names,
                c.State,
                c.Status,
                c.Image
            }));
        }
    }
}
