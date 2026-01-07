using BusinessAsUsual.Admin.Models;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace BusinessAsUsual.Admin.Services
{
    /// <summary>
    /// Provides application environment information, including version, environment name, and configuration sources.
    /// </summary>
    /// <remarks>This service aggregates environment details from the application's configuration and runtime
    /// context. It is typically used to expose diagnostic or informational data about the running application, such as
    /// for health checks, dashboards, or support tools.</remarks>
    public class EnvironmentService
    {
        private readonly IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the EnvironmentService class using the specified configuration settings.
        /// </summary>
        /// <param name="config">The configuration settings to be used by the service. Cannot be null.</param>
        public EnvironmentService(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Retrieves information about the current application environment, including application name, environment,
        /// version, commit details, configuration sources, and loaded secrets.
        /// </summary>
        /// <remarks>Use this method to obtain diagnostic or informational data about the application's
        /// runtime environment, such as for logging, monitoring, or support purposes. The returned information reflects
        /// the state at the time the method is called and may vary depending on deployment and configuration.</remarks>
        /// <returns>An <see cref="EnvironmentInfo"/> object containing details about the application's environment and build
        /// metadata.</returns>
        public EnvironmentInfo GetEnvironmentInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version?.ToString() ?? "Unknown";

            return new EnvironmentInfo
            {
                ApplicationName = assembly.GetName().Name ?? "Unknown",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                Version = version,
                //Commit = ThisAssembly.Git.Commit,
                //CommitDate = ThisAssembly.Git.CommitDate,
                //BuildTime = ThisAssembly.Git.CommitDate,
                ConfigSources = _config.AsEnumerable().Select(c => c.Key).Distinct(),
                SecretsLoaded = GetSecretNames(),
                Containers = GetContainers()
            };
        }

        /// <summary>
        /// Retrieves a collection of Docker containers, including both running and stopped containers.
        /// </summary>
        /// <remarks>Each returned object includes the container's names, image, state, and status
        /// properties. The method returns all containers regardless of their current state. If an error occurs while
        /// retrieving containers, the method returns an empty collection instead of throwing an exception.</remarks>
        /// <returns>An enumerable collection of objects, each containing the names, image, state, and status of a Docker
        /// container. Returns an empty collection if the containers cannot be retrieved.</returns>
        public IEnumerable<object> GetContainers()
        {
            try
            {
                var client = new DockerClientConfiguration().CreateClient();
                var containers = client.Containers.ListContainersAsync(
                    new ContainersListParameters { All = true }
                ).Result;

                return containers.Select(c => new
                {
                    c.Names,
                    c.Image,
                    c.State,
                    c.Status
                });
            }
            catch
            {
                return Enumerable.Empty<object>();
            }
        }

        private IEnumerable<string> GetSecretNames()
        {
            return _config.AsEnumerable()
                .Where(kv => kv.Key.Contains("BAU_"))
                .Select(kv => kv.Key)
                .Distinct();
        }
    }
}
