using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Admin.Models
{
    /// <summary>Company locations (can have more than 1)</summary>
    public class CompanyLocation
    {
        /// <summary>Unique identify for location</summary>
        public Guid Id { get; set; }

        /// <summary>Company Id</summary>
        [Display(Name = "Company Id")]
        [Required(ErrorMessage = "Company Id is required.")]
        public Guid CompanyId { get; set; }

        /// <summary>Name of the location</summary>
        [Display(Name = "Location Name")]
        [Required(ErrorMessage = "Location Name is required.")]
        [MaxLength(100, ErrorMessage = "Location Name must be under 100 characters.")]
        public string LocationName { get; set; } = string.Empty;

        /// <summary>Point of contact for the company</summary>
        [Display(Name = "Contact Name")]
        [MaxLength(100, ErrorMessage = "Contact Name must be under 100 characters.")]
        public string ContactName { get; set; } = string.Empty;

        /// <summary>Contact email</summary>
        [Display(Name = "Contact Email")]
        [MaxLength(100, ErrorMessage = "Contact Email must be under 100 characters.")]
        public string ContactEmail { get; set; } = string.Empty;

        /// <summary>Address Line 1</summary>
        [Display(Name = "Address Line 1")]
        [Required(ErrorMessage = "Address Line 1 is required.")]
        [MaxLength(100, ErrorMessage = "Address Line 1 must be under 100 characters.")]
        public string BillAddr1 { get; set; } = string.Empty;

        /// <summary>Address Line 2</summary>
        [Display(Name = "Address Line 2")]
        [MaxLength(100, ErrorMessage = "Address Line 2 must be under 100 characters.")]
        public string BillAddr2 { get; set; } = string.Empty;

        /// <summary>City</summary>
        [Display(Name = "City")]
        [Required(ErrorMessage = "City is required.")]
        [MaxLength(100, ErrorMessage = "City must be under 100 characters.")]
        public string BillCity { get; set; } = string.Empty;

        /// <summary>State</summary>
        [Display(Name = "State")]
        [Required(ErrorMessage = "State is required.")]
        [MaxLength(100, ErrorMessage = "State must be under 100 characters.")]
        public string BillState { get; set; } = string.Empty;

        /// <summary>Zipcode</summary>
        [Display(Name = "Zipcode")]
        [Required(ErrorMessage = "Zipcode is required.")]
        [MaxLength(25, ErrorMessage = "Zipcode must be under 25 characters.")]
        public string BillZipcode { get; set; } = string.Empty;

        /// <summary>Country</summary>
        [Display(Name = "Country")]
        [Required(ErrorMessage = "Country is required.")]
        [MaxLength(100, ErrorMessage = "Country must be under 100 characters.")]
        public string BillCountry { get; set; } = string.Empty;

        /// <summary>Address Line 1</summary>
        [Display(Name = "Address Line 1")]
        [MaxLength(100, ErrorMessage = "Address Line 1 must be under 100 characters.")]
        public string ShipAddr1 { get; set; } = string.Empty;

        /// <summary>Address Line 2</summary>
        [Display(Name = "Address Line 2")]
        [MaxLength(100, ErrorMessage = "Address Line 2 must be under 100 characters.")]
        public string ShipAddr2 { get; set; } = string.Empty;

        /// <summary>City</summary>
        [Display(Name = "City")]
        [MaxLength(100, ErrorMessage = "City must be under 100 characters.")]
        public string ShipCity { get; set; } = string.Empty;

        /// <summary>State</summary>
        [Display(Name = "State")]
        [MaxLength(100, ErrorMessage = "State must be under 100 characters.")]
        public string ShipState { get; set; } = string.Empty;

        /// <summary>Zipcode</summary>
        [Display(Name = "Zipcode")]
        [MaxLength(25, ErrorMessage = "Zipcode must be under 25 characters.")]
        public string ShipZipcode { get; set; } = string.Empty;

        /// <summary>Country</summary>
        [Display(Name = "Country")]
        [MaxLength(100, ErrorMessage = "Country must be under 100 characters.")]
        public string ShipCountry { get; set; } = string.Empty;

        /// <summary>
        /// UTC timestamp when the company was created.
        /// </summary>
        [Display(Name = "Created At")]
        [Required(ErrorMessage = "Created At is required.")]
        public DateTime CreatedAt { get; set; }
    }
}
