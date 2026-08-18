namespace LMS.Application.Features.Courses.Commands;

public class UnpublishCourseCommand
{
    public Guid CourseId { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
}
