using CRM.Domain.Enums;

namespace CRM.Application.DTOs;

public class OpportunityDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? LeadId { get; set; }
    public string Stage { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Probability { get; set; }
    public decimal WeightedAmount => Amount * (Probability / 100m);
    public DateTime? ExpectedCloseDate { get; set; }
    public DateTime? ActualCloseDate { get; set; }
    public string? Description { get; set; }
    public string? AssignedToEmployeeId { get; set; }
    public string? AssignedToEmployeeName { get; set; }

    // Win/Loss tracking
    public bool IsWon { get; set; }
    public bool IsLost { get; set; }
    public string? LostReason { get; set; }
    public string? CompetitorName { get; set; }

    // Product/Service details
    public string? ProductCategory { get; set; }
    public string? ProductDescription { get; set; }
    public int? Quantity { get; set; }

    // Tracking
    public DateTime CreatedDate { get; set; }
    public DateTime? LastActivityDate { get; set; }
    public int DaysInStage { get; set; }

    public List<string>? Tags { get; set; }
    public int ActivityCount { get; set; }
}

public class CreateOpportunityRequest
{
    public string Name { get; set; } = string.Empty;
    public string? CustomerId { get; set; }
    public string? LeadId { get; set; }
    public OpportunityStage Stage { get; set; } = OpportunityStage.Prospecting;
    public decimal Amount { get; set; }
    public decimal Probability { get; set; } = 50m;
    public DateTime? ExpectedCloseDate { get; set; }
    public string? Description { get; set; }
    public string? AssignedToEmployeeId { get; set; }
    public string? ProductCategory { get; set; }
    public string? ProductDescription { get; set; }
    public int? Quantity { get; set; }
    public List<string>? Tags { get; set; }
}

public class UpdateOpportunityRequest
{
    public string Name { get; set; } = string.Empty;
    public string? CustomerId { get; set; }
    public OpportunityStage Stage { get; set; }
    public decimal Amount { get; set; }
    public decimal Probability { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public string? Description { get; set; }
    public string? AssignedToEmployeeId { get; set; }
    public string? LostReason { get; set; }
    public string? CompetitorName { get; set; }
    public string? ProductCategory { get; set; }
    public string? ProductDescription { get; set; }
    public int? Quantity { get; set; }
    public List<string>? Tags { get; set; }
}
