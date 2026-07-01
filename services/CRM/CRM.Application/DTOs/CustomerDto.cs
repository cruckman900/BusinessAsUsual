namespace CRM.Application.DTOs;

public class CustomerDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Industry { get; set; }
    public int? EmployeeCount { get; set; }
    public decimal? AnnualRevenue { get; set; }
    public string? Description { get; set; }

    // Contact person
    public string? PrimaryContactName { get; set; }
    public string? PrimaryContactEmail { get; set; }
    public string? PrimaryContactPhone { get; set; }
    public string? PrimaryContactTitle { get; set; }

    // Address
    public string? BillingAddressLine1 { get; set; }
    public string? BillingAddressLine2 { get; set; }
    public string? BillingCity { get; set; }
    public string? BillingState { get; set; }
    public string? BillingPostalCode { get; set; }
    public string? BillingCountry { get; set; }

    public string? ShippingAddressLine1 { get; set; }
    public string? ShippingAddressLine2 { get; set; }
    public string? ShippingCity { get; set; }
    public string? ShippingState { get; set; }
    public string? ShippingPostalCode { get; set; }
    public string? ShippingCountry { get; set; }

    // Social
    public string? LinkedInProfile { get; set; }
    public string? TwitterHandle { get; set; }

    // Account management
    public string? AccountManagerEmployeeId { get; set; }
    public string? AccountManagerName { get; set; }
    public string? CustomerSegment { get; set; }
    public string? CustomerStatus { get; set; }

    // Tracking
    public DateTime CreatedDate { get; set; }
    public DateTime? LastContactDate { get; set; }
    public decimal LifetimeValue { get; set; }

    public List<string>? Tags { get; set; }
    public int OpportunityCount { get; set; }
    public int ActivityCount { get; set; }
}

public class CreateCustomerRequest
{
    public string Name { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Industry { get; set; }
    public int? EmployeeCount { get; set; }
    public decimal? AnnualRevenue { get; set; }
    public string? Description { get; set; }

    // Contact person
    public string? PrimaryContactName { get; set; }
    public string? PrimaryContactEmail { get; set; }
    public string? PrimaryContactPhone { get; set; }
    public string? PrimaryContactTitle { get; set; }

    // Address
    public string? BillingAddressLine1 { get; set; }
    public string? BillingAddressLine2 { get; set; }
    public string? BillingCity { get; set; }
    public string? BillingState { get; set; }
    public string? BillingPostalCode { get; set; }
    public string? BillingCountry { get; set; }

    public string? ShippingAddressLine1 { get; set; }
    public string? ShippingAddressLine2 { get; set; }
    public string? ShippingCity { get; set; }
    public string? ShippingState { get; set; }
    public string? ShippingPostalCode { get; set; }
    public string? ShippingCountry { get; set; }

    // Social
    public string? LinkedInProfile { get; set; }
    public string? TwitterHandle { get; set; }

    // Account management
    public string? AccountManagerEmployeeId { get; set; }
    public string? CustomerSegment { get; set; }
    public string? CustomerStatus { get; set; }

    public List<string>? Tags { get; set; }
}

public class UpdateCustomerRequest
{
    public string Name { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Industry { get; set; }
    public int? EmployeeCount { get; set; }
    public decimal? AnnualRevenue { get; set; }
    public string? Description { get; set; }

    // Contact person
    public string? PrimaryContactName { get; set; }
    public string? PrimaryContactEmail { get; set; }
    public string? PrimaryContactPhone { get; set; }
    public string? PrimaryContactTitle { get; set; }

    // Address
    public string? BillingAddressLine1 { get; set; }
    public string? BillingAddressLine2 { get; set; }
    public string? BillingCity { get; set; }
    public string? BillingState { get; set; }
    public string? BillingPostalCode { get; set; }
    public string? BillingCountry { get; set; }

    public string? ShippingAddressLine1 { get; set; }
    public string? ShippingAddressLine2 { get; set; }
    public string? ShippingCity { get; set; }
    public string? ShippingState { get; set; }
    public string? ShippingPostalCode { get; set; }
    public string? ShippingCountry { get; set; }

    // Social
    public string? LinkedInProfile { get; set; }
    public string? TwitterHandle { get; set; }

    // Account management
    public string? AccountManagerEmployeeId { get; set; }
    public string? CustomerSegment { get; set; }
    public string? CustomerStatus { get; set; }

    public decimal LifetimeValue { get; set; }
    public List<string>? Tags { get; set; }
}
