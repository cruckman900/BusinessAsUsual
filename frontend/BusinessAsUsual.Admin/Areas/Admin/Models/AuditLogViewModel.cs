namespace BusinessAsUsual.Admin.Areas.Admin.Models
{
    /// <summary>
    /// View model for displaying company audit logs.
    /// </summary>
    public class AuditLogViewModel
    {
        /// <summary>
        /// Gets or sets the company identifier.
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of audit entries.
        /// </summary>
        public List<AuditEntryViewModel> AuditEntries { get; set; } = new();

        /// <summary>
        /// Gets or sets the filter for event types.
        /// </summary>
        public string? EventTypeFilter { get; set; }

        /// <summary>
        /// Gets or sets the start date filter.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date filter.
        /// </summary>
        public DateTime? EndDate { get; set; }
    }

    /// <summary>
    /// Represents a single audit log entry.
    /// </summary>
    public class AuditEntryViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for this audit entry.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the timestamp of the event.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the type of event (Created, Updated, Archived, etc.).
        /// </summary>
        public string EventType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user who performed the action.
        /// </summary>
        public string PerformedBy { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a description of the change.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets additional details (JSON or text).
        /// </summary>
        public string? Details { get; set; }

        /// <summary>
        /// Gets or sets the display-friendly timestamp.
        /// </summary>
        public string TimestampDisplay => Timestamp.ToString("MMM dd, yyyy h:mm tt");

        /// <summary>
        /// Gets or sets the badge class for the event type.
        /// </summary>
        public string BadgeClass => EventType.ToLower() switch
        {
            "created" => "bg-success",
            "updated" => "bg-info",
            "archived" => "bg-warning text-dark",
            "restored" => "bg-primary",
            "deleted" => "bg-danger",
            "merged" => "bg-secondary",
            _ => "bg-light text-dark"
        };
    }
}
