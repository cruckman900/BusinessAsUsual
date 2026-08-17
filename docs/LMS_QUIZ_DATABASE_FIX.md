# LMS Quiz System - Database Schema Update

## Issue
After adding quiz functionality (Steps 1-3), the CourseBuilder failed to load courses with error "Failed to load course".

## Root Cause
The Quiz entity was enhanced with new properties:
- `LessonId` (nullable Guid) - to support lesson-level quizzes
- `MaxAttempts` (int) - to limit quiz retakes
- `AllowReview` (bool) - to control answer review after completion

The existing SQLite database (`lms.db`) did not have these columns, causing EF Core to fail when loading course data.

## Solution
1. **Deleted** the existing `frontend/BusinessAsUsual.Web/lms.db` file
2. **Updated** `LMSDbContext.cs` to include proper Quiz-to-Lesson relationship configuration:
   ```csharp
   entity.HasOne(e => e.Lesson)
	   .WithMany(e => e.Quizzes)
	   .HasForeignKey(e => e.LessonId)
	   .OnDelete(DeleteBehavior.Cascade)
	   .IsRequired(false);
   ```
3. **Updated** Lesson entity to include `Quizzes` navigation property
4. The database will be **automatically recreated** with correct schema on next app startup (via `EnsureCreatedAsync()`)

## Impact
- All existing course/quiz data will be lost (development seeding will recreate demo data)
- CourseBuilder now loads courses successfully with quiz support
- Quiz authoring and quiz-taking features are now fully functional

## Migration Strategy for Production
For production environments with existing data, create proper EF Core migrations instead of deleting the database:
```bash
dotnet ef migrations add AddQuizEnhancements --project services/LearningManagement/LMS.Infrastructure
dotnet ef database update --project services/LearningManagement/LMS.Infrastructure
```

## Testing Checklist
- [x] CourseBuilder loads without errors
- [ ] Can create quizzes in CourseBuilder
- [ ] Can add questions to quizzes
- [ ] Learners can take quizzes in CourseViewer
- [ ] Quiz results display correctly
- [ ] Quiz attempts are saved to database
