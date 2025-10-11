using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Admin.Areas.Admin.Models
{
    /// <summary>
    /// Represents an employee within the Business As Usual system.
    /// </summary>
    public class Employee
    {
        /// <summary>Unique identifier for the employee</summary>
        public Guid Id { get; set; }

        /// <summary>The company id</summary>
        [Display(Name = "Company Id")]
        [Required(ErrorMessage = "Company Id is required.")]
        public Guid CompanyId { get; set; }

        /// <summary>Employee first name</summary>
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required.")]
        [MaxLength(100, ErrorMessage = "First Name must be under 100 characters.")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>Employee last name</summary>
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required.")]
        [MaxLength(100, ErrorMessage = "Last Name must be under 100 characters.")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>Employee middle name</summary>
        [Display(Name = "Middle Name")]
        [MaxLength(100, ErrorMessage = "Middle Name must be under 100 characters.")]
        public string MiddleName { get; set; } = string.Empty;

        /// <summary>Employee name prefix</summary>
        [Display(Name = "Prefix")]
        [MaxLength(50, ErrorMessage = "Prefix must be under 50 characters.")]
        public string Prefix { get; set; } = string.Empty;

        /// <summary>Employee name suffix</summary>
        [Display(Name = "Suffix")]
        [MaxLength(50, ErrorMessage = "Suffix must be under 50 characters.")]
        public string Suffix { get; set; } = string.Empty;

        /// <summary>Employee role</summary>
        [Display(Name = "Role")]
        [MaxLength(50,ErrorMessage = "Role must be under 50 characters.")]
        public string Role { get; set; } = string.Empty;

        /// <summary>Employee email address</summary>
        [Display(Name = "Email Address")]
        [MaxLength(100, ErrorMessage = "Email Address must be under 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string email { get; set; } = string.Empty;

        /// <summary>
        /// UTC timestamp when the company was created.
        /// </summary>
        [Display(Name = "Created At")]
        [Required(ErrorMessage = "Created At is required.")]
        public DateTime CreatedAt { get; set; }
    }
}