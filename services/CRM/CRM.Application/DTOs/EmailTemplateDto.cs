namespace CRM.Application.DTOs;

/// <summary>
/// A reusable email template with merge-field placeholders (e.g. {{CustomerName}}).
/// </summary>
public class EmailTemplateDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }

    /// <summary>Merge-field tokens referenced by this template (without braces).</summary>
    public List<string> MergeFields { get; set; } = new();
}

public class CreateEmailTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class UpdateEmailTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Well-known merge-field tokens available for personalization. Values are resolved
/// against the target lead / opportunity / customer when an email is composed.
/// </summary>
public static class EmailMergeFields
{
    public const string ContactName = "ContactName";
    public const string CompanyName = "CompanyName";
    public const string OpportunityName = "OpportunityName";
    public const string Amount = "Amount";
    public const string SenderName = "SenderName";

    public static readonly IReadOnlyList<string> All = new[]
    {
        ContactName, CompanyName, OpportunityName, Amount, SenderName
    };
}
