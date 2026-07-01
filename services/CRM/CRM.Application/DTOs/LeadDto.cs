using CRM.Domain.Enums;

namespace CRM.Application.DTOs;

public class LeadDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string? JobTitle { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public decimal? EstimatedValue { get; set; }
    public string? Description { get; set; }
    public string? AssignedToEmployeeId { get; set; }
    public string? AssignedToEmployeeName { get; set; }

    // Address
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }

    // Social
    public string? LinkedInProfile { get; set; }
    public string? TwitterHandle { get; set; }

    // Tracking
    public DateTime CreatedDate { get; set; }
    public DateTime? LastContactedDate { get; set; }
    public DateTime? QualifiedDate { get; set; }
    public DateTime? ConvertedDate { get; set; }
    public string? ConvertedToCustomerId { get; set; }

    public List<string>? Tags { get; set; }
    public int ActivityCount { get; set; }
}

public class CreateLeadRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string? JobTitle { get; set; }
    public LeadSource Source { get; set; } = LeadSource.Website;
    public decimal? EstimatedValue { get; set; }
    public string? Description { get; set; }
    public string? AssignedToEmployeeId { get; set; }

    // Address
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }

    // Social
    public string? LinkedInProfile { get; set; }
    public string? TwitterHandle { get; set; }

    public List<string>? Tags { get; set; }
}

public class UpdateLeadRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string? JobTitle { get; set; }
    public LeadStatus Status { get; set; }
    public LeadSource Source { get; set; }
    public decimal? EstimatedValue { get; set; }
    public string? Description { get; set; }
    public string? AssignedToEmployeeId { get; set; }

    // Address
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }

    // Social
    public string? LinkedInProfile { get; set; }
    public string? TwitterHandle { get; set; }

    public List<string>? Tags { get; set; }
}
