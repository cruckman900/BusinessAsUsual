using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Admin.Models
{
    /// <summary>
    /// Represents a log entry within the Business As Usual system.
    /// </summary>
    public class AuditLogEntry
    {
        /// <summary>Unique identifier for the log entry</summary>
        public Guid Id { get; set; }

        /// <summary>Company Id</summary>
        [Display(Name = "Company Id")]
        [Required(ErrorMessage = "Company Id is required.")]
        public Guid CompanyId { get; set; }

        /// <summary>Entity performing the task</summary>
        [Display(Name = "Actor")]
        [Required(ErrorMessage = "Actor is required.")]
        [MaxLength(100, ErrorMessage = "Actor must be under 100 characters.")]
        public string Actor { get; set; } = string.Empty;

        /// <summary>Action being performed</summary>
        [Display(Name = "Action")]
        [Required(ErrorMessage = "Action is required.")]
        [MaxLength(255, ErrorMessage = "Action must be under 255 characters.")]
        public string Action { get; set; } = string.Empty;

        /// <summary>Entity Id</summary>
        [Display(Name = "Entity Id")]
        public Guid EntityId { get; set; } = Guid.Empty;

        /// <summary>Timestamp action was taken</summary>
        [Display(Name = "Timestamp")]
        [Required(ErrorMessage = "Timestamp cannot be null")]
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
