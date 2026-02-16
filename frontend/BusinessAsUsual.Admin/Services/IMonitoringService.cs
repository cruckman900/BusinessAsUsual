using BusinessAsUsual.Admin.Dtos;

namespace BusinessAsUsual.Admin.Services
{
    /// <summary>
    /// Defines a contract for monitoring the health status of the platform.
    /// </summary>
    /// <remarks>Implementations of this interface should provide mechanisms to assess and report the current
    /// health of the platform's components. This interface is intended to be used by services or controllers that
    /// require up-to-date health information for diagnostics or operational monitoring.</remarks>
    public interface IMonitoringService
    {
        /// <summary>
        /// Asynchronously retrieves the current health status of the platform.
        /// </summary>
        /// <remarks>This method may involve network calls and should be awaited to ensure proper handling
        /// of the asynchronous operation.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see
        /// cref="PlatformHealthDto"/> object that provides details about the platform's health status.</returns>
        Task<PlatformHealthDto> GetPlatformHealthAsync();
    }
}
