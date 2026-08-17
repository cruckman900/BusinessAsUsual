using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly LMSDbContext _context;

    public CourseRepository(LMSDbContext context)
    {
        _context = context;
    }

    public async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
    }

    public async Task<Course?> GetWithModulesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Include(c => c.Modules)
            .ThenInclude(m => m.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
    }

    public async Task<Course?> GetWithFullStructureAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
                    .ThenInclude(l => l.ContentBlocks)
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
                    .ThenInclude(l => l.Quizzes)
                        .ThenInclude(q => q.Questions)
                            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Course>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
            .Where(c => !c.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Course>> GetPublishedCoursesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
            .Where(c => c.Status == CourseStatus.Published && !c.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Course>> GetByAuthorAsync(string authorId, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
            .Where(c => c.CreatedBy == authorId && !c.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<Course> AddAsync(Course course, CancellationToken cancellationToken = default)
    {
        _context.Courses.Add(course);
        await _context.SaveChangesAsync(cancellationToken);
        return course;
    }

    public async Task UpdateAsync(Course course, CancellationToken cancellationToken = default)
    {
        // Load the existing course with all related entities to compare
        var existingCourse = await _context.Courses
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
                    .ThenInclude(l => l.ContentBlocks)
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
                    .ThenInclude(l => l.Quizzes)
                        .ThenInclude(q => q.Questions)
                            .ThenInclude(q => q.Options)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == course.Id, cancellationToken);

        if (existingCourse != null)
        {
            // Find new ContentBlocks (not in existing course)
            var existingBlockIds = existingCourse.Modules
                .SelectMany(m => m.Lessons)
                .SelectMany(l => l.ContentBlocks)
                .Select(b => b.Id)
                .ToHashSet();

            // Find new Quizzes, Questions, and Options
            var existingQuizIds = existingCourse.Modules
                .SelectMany(m => m.Lessons)
                .SelectMany(l => l.Quizzes)
                .Select(q => q.Id)
                .ToHashSet();

            var existingQuestionIds = existingCourse.Modules
                .SelectMany(m => m.Lessons)
                .SelectMany(l => l.Quizzes)
                .SelectMany(q => q.Questions)
                .Select(q => q.Id)
                .ToHashSet();

            var existingOptionIds = existingCourse.Modules
                .SelectMany(m => m.Lessons)
                .SelectMany(l => l.Quizzes)
                .SelectMany(q => q.Questions)
                .SelectMany(q => q.Options)
                .Select(o => o.Id)
                .ToHashSet();

            foreach (var module in course.Modules)
            {
                foreach (var lesson in module.Lessons)
                {
                    // Mark new content blocks as Added
                    foreach (var block in lesson.ContentBlocks)
                    {
                        if (!existingBlockIds.Contains(block.Id))
                        {
                            _context.Entry(block).State = EntityState.Added;
                        }
                    }

                    // Mark new quizzes, questions, and options as Added
                    foreach (var quiz in lesson.Quizzes)
                    {
                        if (!existingQuizIds.Contains(quiz.Id))
                        {
                            _context.Entry(quiz).State = EntityState.Added;
                        }

                        foreach (var question in quiz.Questions)
                        {
                            if (!existingQuestionIds.Contains(question.Id))
                            {
                                _context.Entry(question).State = EntityState.Added;
                            }

                            foreach (var option in question.Options)
                            {
                                if (!existingOptionIds.Contains(option.Id))
                                {
                                    _context.Entry(option).State = EntityState.Added;
                                }
                            }
                        }
                    }
                }
            }
        }

        _context.Courses.Update(course);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await GetByIdAsync(id, cancellationToken);
        if (course != null)
        {
            course.IsDeleted = true;
            await UpdateAsync(course, cancellationToken);
        }
    }
}
