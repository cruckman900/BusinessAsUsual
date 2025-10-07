using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Admin.Models
{
    /// <summary>
    /// Represents a company being provisioned into the system.
    /// </summary>
    public class Company
    {
        /// <summary>Unique identifier for the company.</summary>
        public Guid Id { get; set; }

        /// <summary>Name of the company.</summary>
        [Display(Name = "Company Name")]
        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(100, ErrorMessage = "Name must be under 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Database name assigned to the company.</summary>
        [Display(Name = "Database Name")]
        public string DbName { get; set; } = string.Empty;

        /// <summary>Email address of the company admin.</summary>
        [Display(Name = "Admin Email")]
        [Required(ErrorMessage = "Admin email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string AdminEmail { get; set; } = string.Empty;

        /// <summary>UTC timestamp when the company was created.</summary>
        public DateTime CreatedAt { get; set; }
    }
}