namespace BusinessAsUsual.Admin.Areas.Admin.Models
{
    /// <summary>
    /// Represents disk usage statistics, including total and used disk space values.
    /// </summary>
    public class DiskStats
    {
        /// <summary>
        /// Gets or sets the total value represented by this instance.
        /// </summary>
        public double Total { get; set; }

        /// <summary>
        /// Gets or sets the amount of resource that has been used.
        /// </summary>
        public double Used { get; set; }
    }
}
