using BusinessAsUsual.Admin.Areas.Admin.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessAsUsual.Admin.Services.Metrics
{
    /// <summary>
    /// Provides functionality to collect disk usage statistics for the root filesystem.
    /// </summary>
    /// <remarks>The DiskCollector class is intended for use on Linux systems, where the root filesystem is
    /// represented by "/". It retrieves information about total and used disk space, which can be useful for monitoring
    /// or reporting purposes.</remarks>
    public class DiskCollector
    {
        /// <summary>
        /// Asynchronously retrieves disk usage statistics for the root filesystem.
        /// </summary>
        /// <remarks>This method is intended for use on Linux systems, where the root filesystem is
        /// represented by "/". On other platforms, the returned values may not reflect actual disk usage.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="DiskStats"/> object
        /// with the total and used disk space, in gigabytes, for the root filesystem. If the root drive is not found or
        /// not ready, both values are set to 0.</returns>
        public Task<DiskStats> GetDiskAsync()
        {
            // On Linux, "/" is always the root filesystem
            var drive = DriveInfo.GetDrives()
                .FirstOrDefault(d => d.Name == "/");

            if (drive == null || !drive.IsReady)
            {
                return Task.FromResult(new DiskStats
                {
                    Total = 0,
                    Used = 0
                });
            }

            double totalGb = Math.Round(drive.TotalSize / 1024.0 / 1024.0 / 1024.0, 2);
            double freeGb = Math.Round(drive.TotalFreeSpace / 1024.0 / 1024.0 / 1024.0, 2);
            double usedGb = Math.Round(totalGb - freeGb, 2);

            return Task.FromResult(new DiskStats
            {
                Total = totalGb,
                Used = usedGb
            });
        }
    }
}
