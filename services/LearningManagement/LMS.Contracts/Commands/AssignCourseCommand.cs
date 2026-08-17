namespace LMS.Contracts.Commands;

public class AssignCourseCommand
{
    public Guid CourseId { get; set; }
    public List<string> EmployeeIds { get; set; } = new();
    public DateTime? DueDate { get; set; }
    public string? AssignedBy { get; set; }
    public bool IsMandatory { get; set; } = true;
}
