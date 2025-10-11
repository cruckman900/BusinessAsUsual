using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Admin.Areas.Admin.Models
{
    /// <summary>Company info for clients onboarding to Business As Usual</summary>
    public class CompanyInfo
    {
        /// <summary>Unique identifier for company info</summary>
        public Guid Id { get; set; }

        /// <summary>Name of company</summary>
        [Display(Name = "Company Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Company point of contact</summary>
        [Display(Name = "Contact Name")]
        [Required(ErrorMessage = "Contact Name is required.")]
        [MaxLength(100, ErrorMessage = "Contact Name must be under 100 characters.")]
        public string ContactName { get; set; } = string.Empty;

        /// <summary>Contact Email</summary>
        [Display(Name = "Contact Email")]
        [Required(ErrorMessage = "Contact Email is required.")]
        [MaxLength(100, ErrorMessage = "Contact Email must be under 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string ContactEmail { get; set; } = string.Empty;

        /// <summary>Contact Phone</summary>
        [Display(Name = "Contact Phone")]
        [Required(ErrorMessage = "Contact Phone is required.")]
        [MaxLength(25, ErrorMessage = "Contact Phone must be under 25 characters.")]
        public string ContactPhone { get; set; } = string.Empty;

        /// <summary>
        /// UTC timestamp when the company was created.
        /// </summary>
        [Display(Name = "Created At")]
        [Required(ErrorMessage = "Created At is required.")]
        public DateTime CreatedAt { get; set; }

    }
}
