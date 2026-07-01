namespace CRM.Domain.Entities;

public class Note
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsPinned { get; set; } = false;

    // Relations
    public string? LeadId { get; set; }
    public string? OpportunityId { get; set; }
    public string? CustomerId { get; set; }
    public string? CreatedByEmployeeId { get; set; }

    // Tracking
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }

    // Navigation properties
    public virtual Lead? Lead { get; set; }
    public virtual Opportunity? Opportunity { get; set; }
    public virtual Customer? Customer { get; set; }
}
