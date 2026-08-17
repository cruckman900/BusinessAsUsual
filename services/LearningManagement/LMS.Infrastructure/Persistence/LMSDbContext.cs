using Microsoft.EntityFrameworkCore;
using LMS.Domain.Entities;
using System.Text.Json;

namespace LMS.Infrastructure.Persistence;

public class LMSDbContext : DbContext
{
    public LMSDbContext(DbContextOptions<LMSDbContext> options) : base(options)
    {
    }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<ContentBlock> ContentBlocks => Set<ContentBlock>();
    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
    public DbSet<Answer> Answers => Set<Answer>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<CourseCompletion> CourseCompletions => Set<CourseCompletion>();
    public DbSet<LearnerProgress> LearnerProgresses => Set<LearnerProgress>();

    // New assignment and progress tracking tables
    public DbSet<CourseAssignment> CourseAssignments => Set<CourseAssignment>();
    public DbSet<DetailedLearnerProgress> DetailedLearnerProgress => Set<DetailedLearnerProgress>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Course configuration
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Tags)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());
        });

        // Module configuration
        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.Course)
                .WithMany(e => e.Modules)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Lesson configuration
        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.Module)
                .WithMany(e => e.Lessons)
                .HasForeignKey(e => e.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ContentBlock configuration
        modelBuilder.Entity<ContentBlock>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.JsonContent).IsRequired();
            entity.HasOne(e => e.Lesson)
                .WithMany(e => e.ContentBlocks)
                .HasForeignKey(e => e.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Quiz)
                .WithMany()
                .HasForeignKey(e => e.QuizId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Quiz configuration
        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.Lesson)
                .WithMany(e => e.Quizzes)
                .HasForeignKey(e => e.LessonId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
            entity.HasOne(e => e.Course)
                .WithMany()
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
        });

        // Question configuration
        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuestionText).IsRequired();
            entity.HasOne(e => e.Quiz)
                .WithMany(e => e.Questions)
                .HasForeignKey(e => e.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // QuestionOption configuration
        modelBuilder.Entity<QuestionOption>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OptionText).IsRequired();
            entity.HasOne(e => e.Question)
                .WithMany(e => e.Options)
                .HasForeignKey(e => e.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // QuizAttempt configuration
        modelBuilder.Entity<QuizAttempt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EmployeeId).IsRequired();
            entity.HasOne(e => e.Quiz)
                .WithMany(e => e.Attempts)
                .HasForeignKey(e => e.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Answer configuration
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SelectedOptionIds)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) ?? new List<Guid>());
            entity.HasOne(e => e.QuizAttempt)
                .WithMany(e => e.Answers)
                .HasForeignKey(e => e.QuizAttemptId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Question)
                .WithMany(e => e.Answers)
                .HasForeignKey(e => e.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Assignment configuration
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EmployeeId).IsRequired();
            entity.HasOne(e => e.Course)
                .WithMany(e => e.Assignments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Completion)
                .WithMany()
                .HasForeignKey(e => e.CompletionId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // CourseCompletion configuration
        modelBuilder.Entity<CourseCompletion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EmployeeId).IsRequired();
            entity.Property(e => e.ProgressData).IsRequired();
            entity.HasOne(e => e.Course)
                .WithMany(e => e.Completions)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.FinalAssessmentAttempt)
                .WithMany()
                .HasForeignKey(e => e.FinalAssessmentAttemptId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // LearnerProgress configuration
        modelBuilder.Entity<LearnerProgress>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EmployeeId).IsRequired();
            entity.Property(e => e.CompletedModules)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) ?? new List<Guid>());
            entity.Property(e => e.CompletedLessons)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) ?? new List<Guid>());
            entity.Property(e => e.CompletedQuizzes)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) ?? new List<Guid>());
            entity.HasOne(e => e.Course)
                .WithMany()
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
