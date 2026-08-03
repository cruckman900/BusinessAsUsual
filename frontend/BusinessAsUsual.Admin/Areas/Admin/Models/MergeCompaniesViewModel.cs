using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Admin.Areas.Admin.Models
{
    /// <summary>
    /// View model for merging two companies.
    /// </summary>
    public class MergeCompaniesViewModel
    {
        /// <summary>
        /// Gets or sets the ID of the source company (will be merged into target).
        /// </summary>
        [Required(ErrorMessage = "Source company is required.")]
        public string SourceCompanyId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ID of the target company (will receive the merge).
        /// </summary>
        [Required(ErrorMessage = "Target company is required.")]
        public string TargetCompanyId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether to transfer all users from source to target.
        /// </summary>
        public bool TransferUsers { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to transfer all data from source to target.
        /// </summary>
        public bool TransferData { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to archive the source company after merge.
        /// </summary>
        public bool ArchiveSource { get; set; } = true;

        /// <summary>
        /// Gets or sets any additional notes about the merge operation.
        /// </summary>
        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets the list of available companies for selection.
        /// </summary>
        public List<CompanyListItemViewModel> AvailableCompanies { get; set; } = new();
    }
}
