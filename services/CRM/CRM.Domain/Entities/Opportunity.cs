using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

public class Opportunity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string? CustomerId { get; set; }
    public string? LeadId { get; set; }
    public OpportunityStage Stage { get; set; } = OpportunityStage.Prospecting;
    public decimal Amount { get; set; }
    public decimal Probability { get; set; } = 50m; // Percentage (0-100)
    public DateTime? ExpectedCloseDate { get; set; }
    public DateTime? ActualCloseDate { get; set; }
    public string? Description { get; set; }
    public string? AssignedToEmployeeId { get; set; }

    // Win/Loss tracking
    public bool IsWon => Stage == OpportunityStage.ClosedWon;
    public bool IsLost => Stage == OpportunityStage.ClosedLost;
    public bool IsClosed => IsWon || IsLost;
    public string? LostReason { get; set; }
    public string? CompetitorName { get; set; }

    // Product/Service details
    public string? ProductCategory { get; set; }
    public string? ProductDescription { get; set; }
    public int? Quantity { get; set; }

    // Tracking
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastActivityDate { get; set; }
    public int DaysInStage => (DateTime.UtcNow - CreatedDate).Days;

    // Metadata
    public Dictionary<string, string>? CustomFields { get; set; }
    public List<string>? Tags { get; set; }

    // Navigation properties
    public virtual Customer? Customer { get; set; }
    public virtual Lead? Lead { get; set; }
    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
}
