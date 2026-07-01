using CRM.Domain.Enums;

namespace CRM.Application.DTOs;

public class ActivityDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ActivityDate { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string Priority { get; set; } = string.Empty;
    public int? DurationMinutes { get; set; }

    // Relations
    public string? LeadId { get; set; }
    public string? OpportunityId { get; set; }
    public string? CustomerId { get; set; }
    public string? AssignedToEmployeeId { get; set; }
    public string? AssignedToEmployeeName { get; set; }

    // Outcome
    public string? Outcome { get; set; }
    public string? NextSteps { get; set; }

    public DateTime CreatedDate { get; set; }
}

public class CreateActivityRequest
{
    public ActivityType Type { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ActivityDate { get; set; }
    public DateTime? DueDate { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public int? DurationMinutes { get; set; }

    // Relations - at least one must be provided
    public string? LeadId { get; set; }
    public string? OpportunityId { get; set; }
    public string? CustomerId { get; set; }
    public string? AssignedToEmployeeId { get; set; }
}

public class UpdateActivityRequest
{
    public ActivityType Type { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ActivityDate { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedDate { get; set; }
    public Priority Priority { get; set; }
    public int? DurationMinutes { get; set; }
    public string? AssignedToEmployeeId { get; set; }
    public string? Outcome { get; set; }
    public string? NextSteps { get; set; }
}
