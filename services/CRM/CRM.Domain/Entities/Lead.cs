using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

public class Lead
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string? JobTitle { get; set; }
    public LeadStatus Status { get; set; } = LeadStatus.New;
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

    // Tracking
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastContactedDate { get; set; }
    public DateTime? QualifiedDate { get; set; }
    public DateTime? ConvertedDate { get; set; }
    public string? ConvertedToCustomerId { get; set; }

    // Metadata
    public Dictionary<string, string>? CustomFields { get; set; }
    public List<string>? Tags { get; set; }

    // Navigation properties
    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
}
