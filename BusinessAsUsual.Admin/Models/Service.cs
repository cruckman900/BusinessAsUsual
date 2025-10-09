using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Admin.Models
{
    /// <summary>
    /// Represents a product within the Business As Usual system.
    /// </summary>
    public class Product
    {
        /// <summary>Product Id</summary>
        public Guid Id { get; set; }

        /// <summary>Company Id</summary>
        [Display(Name = "Company Id")]
        [Required(ErrorMessage = "Company Id is required.")]
        public Guid CompanyId { get; set; }

        /// <summary>Service name</summary>
        [Display(Name = "Service Name")]
        [Required(ErrorMessage = "Service Name is required.")]
        [MaxLength(100, ErrorMessage = "Service Name must be under 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Product description/// </summary>
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>Price of the product</summary>
        [Display(Name = "Price")]
        [Required(ErrorMessage = "Price is required.")]
        public double Price { get; set; } = 0;

        /// <summary>
        /// UTC timestamp when the company was created.
        /// </summary>
        [Display(Name = "Created At")]
        [Required(ErrorMessage = "Created At is required.")]
        public DateTime CreatedAt { get; set; }
    }
}
