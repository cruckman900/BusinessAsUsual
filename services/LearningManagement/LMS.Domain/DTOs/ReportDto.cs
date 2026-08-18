namespace LMS.Domain.DTOs;

public class ReportTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class GenerateReportRequest
{
    public Guid? TemplateId { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Dictionary<string, string> Filters { get; set; } = new();
}
