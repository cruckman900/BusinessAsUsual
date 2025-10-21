using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Web.Data.Entities
{
    /// <summary>
    /// Represents an individual employee within a company.
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Unique identifier for the employee.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key referencing the owning company.
        /// </summary>
        [Required]
        public Guid CompanyId { get; set; }

        /// <summary>
        /// First name of the employee.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Last name of the employee.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Middle name of the employee.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string MiddleName { get; set; } = string.Empty;

        /// <summary>
        /// Prefix for the employee's name (e.g., Mr., Ms., Dr.).
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Prefix { get; set; } = string.Empty;

        /// <summary>
        /// Suffix for the employee's name (e.g., Jr., Sr., III).
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Suffix { get; set; } = string.Empty;

        /// <summary>
        /// Role assigned to the employee (e.g., Admin, Manager).
        /// </summary>
        [MaxLength(50)]
        public string? Role { get; set; }

        /// <summary>
        /// Email address used for communication and login.
        /// </summary>
        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// Sets whether the employee is currently active.
        /// </summary>
        [Required]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Timestamp when the employee record was created.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when the employee record was last updated.
        /// </summary>
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}