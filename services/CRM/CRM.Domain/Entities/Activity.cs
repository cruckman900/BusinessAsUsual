using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

public class Activity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public ActivityType Type { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ActivityDate { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletedDate { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public int? DurationMinutes { get; set; }

    // Relations
    public string? LeadId { get; set; }
    public string? OpportunityId { get; set; }
    public string? CustomerId { get; set; }
    public string? AssignedToEmployeeId { get; set; }

    // Outcome
    public string? Outcome { get; set; }
    public string? NextSteps { get; set; }

    // Tracking
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Lead? Lead { get; set; }
    public virtual Opportunity? Opportunity { get; set; }
    public virtual Customer? Customer { get; set; }
}
