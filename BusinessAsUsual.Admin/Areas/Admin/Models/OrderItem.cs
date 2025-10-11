using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Admin.Areas.Admin.Models
{
    /// <summary>
    /// Represents a line item as defined in Business As Usual
    /// </summary>
    public class OrderItem
    {
        /// <summary>Line Item Id</summary>
        public Guid Id { get; set; }

        /// <summary>Company Id</summary>
        [Display(Name = "Company Id")]
        [Required(ErrorMessage = "Company Id is required.")]
        public Guid CompanyId { get; set; }

        /// <summary>Order Id</summary>
        [Display(Name = "Order Id")]
        [Required(ErrorMessage = "Order Id is required.")]
        public Guid OrderId { get; set; }

        /// <summary>Item Type -- Product or Service</summary>
        [Display(Name = "Item Type")]
        [Required(ErrorMessage = "Item Type is required.")]
        [MaxLength(50, ErrorMessage = "Item Type must be less than 50 characters.")]
        public string ItemType { get; set; } = string.Empty;

        /// <summary>Product or Service Id</summary>
        [Display(Name = "Item Id")]
        [Required(ErrorMessage = "Item Id is required.")]
        public Guid ItemId { get; set; }

        /// <summary>Name of product or service</summary>
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(50, ErrorMessage = "Name must be less than 50 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Quantity</summary>
        [Display(Name = "Quantity")]
        [Required(ErrorMessage = "Quantity is required.")]
        public int Quantity { get; set; } = 1;

        /// <summary>Price per unit</summary>
        [Display(Name = "Unit Price")]
        [Required(ErrorMessage = "Unit Price is required.")]
        public double UnitPrice { get; set; } = 0;

        /// <summary>Discount Amount</summary>
        [Display(Name = "Discount Amount")]
        [Required(ErrorMessage = "Discount Amount is required.")]
        public double DiscountAmount { get; set; } = 0;

        /// <summary>Tax Amount</summary>
        [Display(Name = "Tax Amount")]
        [Required(ErrorMessage = "Tax Amount is required.")]
        public double TaxAmount { get; set; } = 0;

        /// <summary>Line item total</summary>
        [Display(Name = "Total")]
        [Required(ErrorMessage = "Total is required.")]
        public double Total { get; set; } = 0;
    }
}
