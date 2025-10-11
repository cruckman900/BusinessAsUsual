using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Admin.Areas.Admin.Models
{
    /// <summary>
    /// Fulfillment as defined by Business As Usual
    /// </summary>
    public class Fulfillment
    {
        /// <summary>Fulfillment Id</summary>
        public Guid Id { get; set; }

        /// <summary>Company Id</summary>
        [Display(Name = "Company id")]
        [Required(ErrorMessage = "Company Id is required.")]
        public Guid CompanyId { get; set; }

        /// <summary>Order Id</summary>
        [Display(Name = "Order Id")]
        [Required(ErrorMessage = "Order Id is required.")]
        public Guid OrderId { get; set; }

        /// <summary>Date of Fulfillment</summary>
        [Display(Name = "Fulfillment Date")]
        public DateTime FulfilledAt { get; set; } = DateTime.Now;

        /// <summary>Entity that fulfilled the order</summary>
        [Display(Name = "Fulfilled By")]
        [MaxLength(100, ErrorMessage = "Fulfilled By must be less than 100 characters.")]
        public string FulfilledBy { get; set; } = string.Empty;

        /// <summary>Fulfillment Notes</summary>
        [Display(Name = "Notes")]
        public string Notes { get; set; } = string.Empty;
    }
}
