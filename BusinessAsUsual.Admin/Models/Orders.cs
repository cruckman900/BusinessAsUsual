using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Admin.Models
{
    /// <summary>
    /// Orders as defined in Businss As Usual
    /// </summary>
    public class Orders
    {
        /// <summary>Order Id</summary>
        public Guid Id { get; set; }

        /// <summary>Company Id</summary>
        [Display(Name = "Company Id")]
        [Required(ErrorMessage = "Company Id is required.")]
        public Guid CompanyId { get; set; }

        /// <summary>Customer Id</summary>
        [Display(Name = "Customer Id")]
        [Required(ErrorMessage = "Customer Id is required.")]
        public Guid CustomerId { get; set; }

        /// <summary>Date ordered</summary>
        [Display(Name = "Order Date")]
        [Required(ErrorMessage = "Order Date is required.")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        /// <summary>Order Status</summary>
        [Display(Name = "Order Status")]
        [Required(ErrorMessage = "Order Status is required.")]
        [MaxLength(50, ErrorMessage = "Order Status must be less than 50 characters.")]
        public string Status { get; set; } = "Pending";

        /// <summary>Discount Amount</summary>
        [Display(Name = "Discount Amount")]
        [Required(ErrorMessage = "Discount Amount is required.")]
        public double DiscountAmount { get; set; } = 0;

        /// <summary>Tax Amount</summary>
        [Display(Name = "Tax Amount")]
        [Required(ErrorMessage = "Tax Amount is required.")]
        public double TaxAmount { get; set; } = 0;

        /// <summary>Cost for shipping</summary>
        [Display(Name = "Shipping & Handling")]
        [Required(ErrorMessage = "Shipping & Handling is required.")]
        public double ShippingAmount { get; set; } = 0;

        /// <summary>Fulfillment Status of the order</summary>
        [Display(Name = "Fulfillment Status")]
        [Required(ErrorMessage = "Fulfillment Status is required.")]
        [MaxLength(50, ErrorMessage = "Fulfillment Status must be less than 50 characters.")]
        public string FulfillmentSatus { get; set; } = "Unfulfilled";

        /// <summary>Total cost of the order</summary>
        [Display(Name = "Total")]
        [Required(ErrorMessage = "Total is required.")]
        public double Total { get; set; } = 0;
    }
}
