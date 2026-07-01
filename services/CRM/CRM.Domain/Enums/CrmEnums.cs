namespace CRM.Domain.Enums;

public enum LeadStatus
{
    New = 0,
    Contacted = 1,
    Qualified = 2,
    Unqualified = 3,
    Converted = 4
}

public enum LeadSource
{
    Website = 0,
    Referral = 1,
    SocialMedia = 2,
    EmailCampaign = 3,
    Event = 4,
    Partner = 5,
    DirectCall = 6,
    Other = 7
}

public enum OpportunityStage
{
    Prospecting = 0,
    Qualification = 1,
    NeedsAnalysis = 2,
    Proposal = 3,
    Negotiation = 4,
    ClosedWon = 5,
    ClosedLost = 6
}

public enum ActivityType
{
    Call = 0,
    Email = 1,
    Meeting = 2,
    Task = 3,
    Note = 4,
    Demo = 5,
    Proposal = 6
}

public enum Priority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Urgent = 3
}
