using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Admin.Models
{
    /// <summary>
    /// Represents a billing record within the Business As Usual systeml
    /// </summary>
    public class BillingHistoryRecord
    {
        /// <summary>Unique identifier for the billing record</summary>
        public Guid Id { get; set; }

        /// <summary>Company Id</summary>
        [Display(Name = "Company Id")]
        [Required(ErrorMessage = "Company Id is required.")]
        public Guid CompanyId { get; set; }

        /// <summary>The billed amount</summary>
        [Display(Name = "Billing Amount")]
        [Required(ErrorMessage = "Billing Amount is required.")]
        public double Amount { get; set; } = 0;

        /// <summary>Billing description</summary>
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        [MaxLength(255, ErrorMessage = "Description must be under 255 characters.")]
        public string Description { get; set; } = string.Empty;

        /// <summary>Date the bill is issued</summary>
        [Display(Name = "Billing Date")]
        [Required(ErrorMessage = "Billing Date is required.")]
        public DateTime BilledAt { get; set; } = DateTime.Now;

        /// <summary>Date bill was paid</summary>
        [Display(Name = "Paid On Date")]
        public DateTime PaidOn { get; set; }
    }
}
