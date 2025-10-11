using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Admin.Areas.Admin.Models
{
    /// <summary>Represents the usage of a module per company inside Business As Usual system.
    /// </summary>
    public class ModuleUsage
    {
        /// <summary>Unique identifier for a module usage count</summary>
        public Guid Id { get; set; }

        /// <summary>Company Id</summary>
        [Display(Name = "Company Id")]
        [Required(ErrorMessage = "Company Id is required.")]
        public Guid CompanyId { get; set; }

        /// <summary>Name of the module being tracked</summary>
        [Display(Name = "Module Name")]
        [Required(ErrorMessage = "Module Name is required.")]
        [MaxLength(100, ErrorMessage = "Module Name must be under 100 characters.")]
        public string ModuleName { get; set; } = string.Empty;

        /// <summary>Usage count</summary>
        [Display(Name = "Usage Count")]
        [Required(ErrorMessage = "Usage Count is required.")]
        public int UsageCount { get; set; } = 0;

        /// <summary>Last used date</summary>
        [Display(Name = "Last Used")]
        [Required(ErrorMessage = "Last Used is required.")]
        public DateTime LastUsed { get; set; } = DateTime.Now;
    }
}
