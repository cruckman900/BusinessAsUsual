namespace BusinessAsUsual.Admin.Models
{
    /// <summary>
    /// Represents a provisioned company and its metadata.
    /// </summary>
    public class Company
    {
        /// <summary>Unique identifier for the company.</summary>
        public Guid Id { get; set; }

        /// <summary>Name of the company.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Database name assigned to the company.</summary>
        public string DbName { get; set; } = string.Empty;

        /// <summary>Admin email for the company.</summary>
        public string AdminEmail {  get; set; } = string.Empty;

        /// <summary>UTC timestamp when the company was provisioned.</summary>
        public DateTime CreatedAt { get; set; }
    }
}
