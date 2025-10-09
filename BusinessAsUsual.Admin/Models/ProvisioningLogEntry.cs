using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Admin.Models
{
    /// <summary>Provisioning Log Entry</summary>
    public class ProvisioningLogEntry
    {
        /// <summary>Entry Id</summary>
        public Guid Id { get; set; }

        /// <summary>Company Id</summary>
        [Display(Name = "Company Id")]
        [Required(ErrorMessage = "Company Id cannot be null")]
        public Guid CompanyId { get; set; }

        /// <summary>Provisioning step being performed</summary>
        [Display(Name = "Provisioning Step")]
        [MaxLength(100, ErrorMessage = "Step must be under 100 characters.")]
        public string Step { get; set; } = string.Empty;

        /// <summary>Status of the step being performed</summary>
        [Display(Name = "Status")]
        [MaxLength(50, ErrorMessage = "Status must be under 100 characters.")]
        public string Status { get; set; } = string.Empty;

        /// <summary>Message relating to the step being performed</summary>
        [Display(Name = "Message")]
        [MaxLength(255, ErrorMessage = "Message must be under 255 characters.")]
        public string Message { get; set; } = string.Empty;

        /// <summary>Timestamp step was taken</summary>
        [Display(Name = "Timestamp")]
        [Required(ErrorMessage = "Timestamp cannot be null")]
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
