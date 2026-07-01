namespace CRM.Application.DTOs;

public class NoteDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsPinned { get; set; }

    // Relations
    public string? LeadId { get; set; }
    public string? OpportunityId { get; set; }
    public string? CustomerId { get; set; }
    public string? CreatedByEmployeeId { get; set; }
    public string? CreatedByEmployeeName { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}

public class CreateNoteRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsPinned { get; set; } = false;

    // Relations - at least one must be provided
    public string? LeadId { get; set; }
    public string? OpportunityId { get; set; }
    public string? CustomerId { get; set; }
    public string? CreatedByEmployeeId { get; set; }
}

public class UpdateNoteRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
}
