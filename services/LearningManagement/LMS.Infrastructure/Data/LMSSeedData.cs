using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LMS.Infrastructure.Data;

/// <summary>
/// Seeds the LMS database with realistic demo data
/// </summary>
public class LMSSeedData
{
    private readonly LMSDbContext _context;
    private readonly ILogger<LMSSeedData> _logger;

    public LMSSeedData(LMSDbContext context, ILogger<LMSSeedData> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        // Check if data already exists
        if (await _context.Courses.AnyAsync())
        {
            _logger.LogInformation("Database already seeded, skipping...");
            return;
        }

        _logger.LogInformation("Seeding LMS database with demo data...");

        // Create courses
        var courses = CreateCourses();
        await _context.Courses.AddRangeAsync(courses);
        await _context.SaveChangesAsync();

        // Create employees
        var employees = CreateMockEmployees();

        // Create progress and completions
        await CreateProgressAndCompletions(courses, employees);

        // Create new assignment and progress tracking data
        await CreateAssignmentsAndProgress(courses, employees);

        // Create demo certificates
        await CreateCertificates(courses, employees);

        // Create demo notifications
        await CreateNotifications(courses, employees);

        _logger.LogInformation("LMS database seeded successfully!");
    }

    private List<Course> CreateCourses()
    {
        var courses = new List<Course>();

        // Course 1: C# Fundamentals
        var csharpCourse = CreateCSharpFundamentalsCourse();
        courses.Add(csharpCourse);

        // Course 2: Agile Project Management
        var agileCourse = CreateAgileProjectManagementCourse();
        courses.Add(agileCourse);

        // Course 3: Database Design Essentials
        var databaseCourse = CreateDatabaseDesignCourse();
        courses.Add(databaseCourse);

        return courses;
    }

    private Course CreateCSharpFundamentalsCourse()
    {
        var course = new Course
        {
            Title = "C# Fundamentals for Beginners",
            Description = "Master the basics of C# programming! This comprehensive course covers variables, data types, control structures, object-oriented programming, and best practices. Perfect for developers starting their journey with .NET.",
            Category = "Software Development",
            Tags = new List<string> { "C#", "Programming", ".NET", "Beginner" },
            Difficulty = CourseDifficulty.Beginner,
            EstimatedDurationMinutes = 240,
            Status = CourseStatus.Published,
            PublishedDate = DateTime.UtcNow.AddDays(-30),
            RequiresAssessment = true,
            PassingScore = 75,
            MaxAttempts = 3,
            IssuesCertificate = true,
            CertificateValidityDays = 365,
            CreatedBy = "admin",
            CreatedAt = DateTime.UtcNow.AddDays(-35)
        };

        // Module 1: Getting Started
        var module1 = new Module
        {
            Title = "Getting Started with C#",
            Description = "Introduction to C# and setting up your development environment",
            OrderIndex = 0,
            CreatedAt = DateTime.UtcNow.AddDays(-35)
        };

        var lesson1_1 = new Lesson
        {
            Title = "What is C# and .NET?",
            Description = "Understanding the C# language and .NET ecosystem",
            OrderIndex = 0,
            EstimatedDurationMinutes = 15,
            CreatedAt = DateTime.UtcNow.AddDays(-35)
        };

        lesson1_1.ContentBlocks.Add(new ContentBlock
        {
            BlockType = ContentBlockType.Heading,
            OrderIndex = 0,
            JsonContent = JsonSerializer.Serialize(new { text = "Welcome to C# Programming!", level = 1 }),
            CreatedAt = DateTime.UtcNow.AddDays(-35)
        });

        lesson1_1.ContentBlocks.Add(new ContentBlock
        {
            BlockType = ContentBlockType.Text,
            OrderIndex = 1,
            JsonContent = JsonSerializer.Serialize(new { 
                content = "C# (pronounced 'C Sharp') is a modern, object-oriented programming language developed by Microsoft. It's widely used for building Windows applications, web services, games, and cloud-based solutions." 
            }),
            CreatedAt = DateTime.UtcNow.AddDays(-35)
        });

        lesson1_1.ContentBlocks.Add(new ContentBlock
        {
            BlockType = ContentBlockType.Callout,
            OrderIndex = 2,
            JsonContent = JsonSerializer.Serialize(new { 
                type = "info",
                title = "Did You Know?",
                content = "C# was created by Anders Hejlsberg and first released in 2000. It's now one of the most popular programming languages in the world!"
            }),
            CreatedAt = DateTime.UtcNow.AddDays(-35)
        });

        module1.Lessons.Add(lesson1_1);

        var lesson1_2 = new Lesson
        {
            Title = "Setting Up Visual Studio",
            Description = "Installing and configuring your development environment",
            OrderIndex = 1,
            EstimatedDurationMinutes = 20,
            CreatedAt = DateTime.UtcNow.AddDays(-35)
        };

        lesson1_2.ContentBlocks.Add(new ContentBlock
        {
            BlockType = ContentBlockType.Text,
            OrderIndex = 0,
            JsonContent = JsonSerializer.Serialize(new { 
                content = "Visual Studio is the premier IDE for C# development. Let's get it set up!" 
            }),
            CreatedAt = DateTime.UtcNow.AddDays(-35)
        });

        module1.Lessons.Add(lesson1_2);

        course.Modules.Add(module1);

        // Module 2: Variables and Data Types
        var module2 = new Module
        {
            Title = "Variables and Data Types",
            Description = "Learn about different data types and how to work with variables",
            OrderIndex = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-35)
        };

        var lesson2_1 = new Lesson
        {
            Title = "Understanding Variables",
            Description = "What are variables and how to declare them",
            OrderIndex = 0,
            EstimatedDurationMinutes = 25,
            CreatedAt = DateTime.UtcNow.AddDays(-35)
        };

        lesson2_1.ContentBlocks.Add(new ContentBlock
        {
            BlockType = ContentBlockType.CodeSnippet,
            OrderIndex = 0,
            JsonContent = JsonSerializer.Serialize(new { 
                language = "csharp",
                code = "// Declaring variables in C#\nint age = 25;\nstring name = \"John\";\nbool isActive = true;\ndouble salary = 75000.50;"
            }),
            CreatedAt = DateTime.UtcNow.AddDays(-35)
        });

        module2.Lessons.Add(lesson2_1);
        course.Modules.Add(module2);

        // Module 3: Object-Oriented Programming
        var module3 = new Module
        {
            Title = "Object-Oriented Programming",
            Description = "Master the fundamentals of OOP in C#",
            OrderIndex = 2,
            CreatedAt = DateTime.UtcNow.AddDays(-35)
        };

        var lesson3_1 = new Lesson
        {
            Title = "Classes and Objects",
            Description = "Introduction to classes and objects",
            OrderIndex = 0,
            EstimatedDurationMinutes = 30,
            CreatedAt = DateTime.UtcNow.AddDays(-35)
        };

        lesson3_1.ContentBlocks.Add(new ContentBlock
        {
            BlockType = ContentBlockType.Text,
            OrderIndex = 0,
            JsonContent = JsonSerializer.Serialize(new { 
                content = "Classes are blueprints for creating objects. They define the properties and methods that objects will have."
            }),
            CreatedAt = DateTime.UtcNow.AddDays(-35)
        });

        module3.Lessons.Add(lesson3_1);
        course.Modules.Add(module3);

        return course;
    }

    private Course CreateAgileProjectManagementCourse()
    {
        var course = new Course
        {
            Title = "Agile Project Management Essentials",
            Description = "Learn the core principles of Agile methodology and how to effectively manage projects using Scrum, Kanban, and other Agile frameworks. Perfect for project managers, team leads, and anyone involved in software development.",
            Category = "Project Management",
            Tags = new List<string> { "Agile", "Scrum", "Kanban", "Project Management" },
            Difficulty = CourseDifficulty.Intermediate,
            EstimatedDurationMinutes = 180,
            Status = CourseStatus.Published,
            PublishedDate = DateTime.UtcNow.AddDays(-20),
            RequiresAssessment = true,
            PassingScore = 80,
            MaxAttempts = 3,
            IssuesCertificate = true,
            CertificateValidityDays = 730,
            CreatedBy = "admin",
            CreatedAt = DateTime.UtcNow.AddDays(-25)
        };

        // Module 1: Agile Fundamentals
        var module1 = new Module
        {
            Title = "Introduction to Agile",
            Description = "Understanding Agile principles and values",
            OrderIndex = 0,
            CreatedAt = DateTime.UtcNow.AddDays(-25)
        };

        var lesson1_1 = new Lesson
        {
            Title = "The Agile Manifesto",
            Description = "Exploring the four values and twelve principles",
            OrderIndex = 0,
            EstimatedDurationMinutes = 30,
            CreatedAt = DateTime.UtcNow.AddDays(-25)
        };

        lesson1_1.ContentBlocks.Add(new ContentBlock
        {
            BlockType = ContentBlockType.Heading,
            OrderIndex = 0,
            JsonContent = JsonSerializer.Serialize(new { text = "The Four Values of Agile", level = 2 }),
            CreatedAt = DateTime.UtcNow.AddDays(-25)
        });

        lesson1_1.ContentBlocks.Add(new ContentBlock
        {
            BlockType = ContentBlockType.Text,
            OrderIndex = 1,
            JsonContent = JsonSerializer.Serialize(new { 
                content = "The Agile Manifesto emphasizes: Individuals and interactions over processes and tools, Working software over comprehensive documentation, Customer collaboration over contract negotiation, and Responding to change over following a plan."
            }),
            CreatedAt = DateTime.UtcNow.AddDays(-25)
        });

        module1.Lessons.Add(lesson1_1);
        course.Modules.Add(module1);

        // Module 2: Scrum Framework
        var module2 = new Module
        {
            Title = "Scrum Framework",
            Description = "Deep dive into Scrum roles, events, and artifacts",
            OrderIndex = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-25)
        };

        var lesson2_1 = new Lesson
        {
            Title = "Scrum Roles",
            Description = "Product Owner, Scrum Master, and Development Team",
            OrderIndex = 0,
            EstimatedDurationMinutes = 25,
            CreatedAt = DateTime.UtcNow.AddDays(-25)
        };

        lesson2_1.ContentBlocks.Add(new ContentBlock
        {
            BlockType = ContentBlockType.Text,
            OrderIndex = 0,
            JsonContent = JsonSerializer.Serialize(new { 
                content = "Scrum defines three key roles: The Product Owner who maximizes value, the Scrum Master who facilitates the process, and the Development Team who builds the product."
            }),
            CreatedAt = DateTime.UtcNow.AddDays(-25)
        });

        module2.Lessons.Add(lesson2_1);
        course.Modules.Add(module2);

        return course;
    }

    private Course CreateDatabaseDesignCourse()
    {
        var course = new Course
        {
            Title = "Database Design Essentials",
            Description = "Master the art of database design! Learn about entity-relationship modeling, normalization, indexing strategies, and best practices for creating efficient, scalable database schemas.",
            Category = "Data & Analytics",
            Tags = new List<string> { "Database", "SQL", "Design", "Data Modeling" },
            Difficulty = CourseDifficulty.Intermediate,
            EstimatedDurationMinutes = 200,
            Status = CourseStatus.Published,
            PublishedDate = DateTime.UtcNow.AddDays(-15),
            RequiresAssessment = true,
            PassingScore = 75,
            MaxAttempts = 3,
            IssuesCertificate = true,
            CertificateValidityDays = 547,
            CreatedBy = "admin",
            CreatedAt = DateTime.UtcNow.AddDays(-20)
        };

        // Module 1: Database Fundamentals
        var module1 = new Module
        {
            Title = "Database Fundamentals",
            Description = "Introduction to database concepts",
            OrderIndex = 0,
            CreatedAt = DateTime.UtcNow.AddDays(-20)
        };

        var lesson1_1 = new Lesson
        {
            Title = "What is a Database?",
            Description = "Understanding relational databases",
            OrderIndex = 0,
            EstimatedDurationMinutes = 20,
            CreatedAt = DateTime.UtcNow.AddDays(-20)
        };

        lesson1_1.ContentBlocks.Add(new ContentBlock
        {
            BlockType = ContentBlockType.Text,
            OrderIndex = 0,
            JsonContent = JsonSerializer.Serialize(new { 
                content = "A database is an organized collection of structured data. Relational databases organize data into tables with rows and columns, connected through relationships."
            }),
            CreatedAt = DateTime.UtcNow.AddDays(-20)
        });

        module1.Lessons.Add(lesson1_1);
        course.Modules.Add(module1);

        return course;
    }

    private List<MockEmployee> CreateMockEmployees()
    {
        return new List<MockEmployee>
        {
            new() { Id = "admin", Name = "Admin User", Department = "Administration", JobTitle = "System Administrator" },
            new() { Id = "EMP001", Name = "Sarah Johnson", Department = "Engineering", JobTitle = "Senior Developer" },
            new() { Id = "EMP002", Name = "Michael Chen", Department = "Engineering", JobTitle = "Junior Developer" },
            new() { Id = "EMP003", Name = "Emily Rodriguez", Department = "Project Management", JobTitle = "Project Manager" },
            new() { Id = "EMP004", Name = "David Kim", Department = "Engineering", JobTitle = "Database Administrator" },
            new() { Id = "EMP005", Name = "Lisa Anderson", Department = "HR", JobTitle = "HR Manager" }
        };
    }

    private async Task CreateProgressAndCompletions(List<Course> courses, List<MockEmployee> employees)
    {
        var csharpCourse = courses[0];
        var agileCourse = courses[1];
        var databaseCourse = courses[2];

        // Sarah Johnson - Completed C# course
        var sarahCompletion = new CourseCompletion
        {
            CourseId = csharpCourse.Id,
            EmployeeId = "EMP001",
            StartedAt = DateTime.UtcNow.AddDays(-10),
            CompletedAt = DateTime.UtcNow.AddDays(-2),
            FinalScore = 92,
            Passed = true,
            CertificateIssued = true,
            CertificateUrl = "/certificates/sarah-csharp-fundamentals.pdf",
            CertificateIssuedDate = DateTime.UtcNow.AddDays(-2),
            CertificateExpiryDate = DateTime.UtcNow.AddDays(363),
            ProgressData = JsonSerializer.Serialize(new { completedModules = new[] { csharpCourse.Modules.Select(m => m.Id) } }),
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        };
        await _context.CourseCompletions.AddAsync(sarahCompletion);

        // Michael Chen - 60% through C# course
        var module0 = csharpCourse.Modules.ElementAt(0);
        var module1 = csharpCourse.Modules.ElementAt(1);
        var michaelProgress = new LearnerProgress
        {
            EmployeeId = "EMP002",
            CourseId = csharpCourse.Id,
            CurrentModuleId = module1.Id,
            CurrentLessonId = module1.Lessons.FirstOrDefault()?.Id,
            CompletedModules = new List<Guid> { module0.Id },
            CompletedLessons = module0.Lessons.Select(l => l.Id).ToList(),
            ProgressPercentage = 60,
            LastAccessedAt = DateTime.UtcNow.AddHours(-5),
            CreatedAt = DateTime.UtcNow.AddDays(-7)
        };
        await _context.LearnerProgresses.AddAsync(michaelProgress);

        // Emily Rodriguez - 80% through Agile course
        var agileModule0 = agileCourse.Modules.ElementAt(0);
        var agileModule1 = agileCourse.Modules.ElementAt(1);
        var emilyProgress = new LearnerProgress
        {
            EmployeeId = "EMP003",
            CourseId = agileCourse.Id,
            CurrentModuleId = agileModule1.Id,
            CurrentLessonId = agileModule1.Lessons.FirstOrDefault()?.Id,
            CompletedModules = new List<Guid> { agileModule0.Id },
            CompletedLessons = agileModule0.Lessons.Select(l => l.Id).ToList(),
            ProgressPercentage = 80,
            LastAccessedAt = DateTime.UtcNow.AddHours(-2),
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        };
        await _context.LearnerProgresses.AddAsync(emilyProgress);

        // David Kim - Just enrolled in Database course
        var dbModule0 = databaseCourse.Modules.FirstOrDefault();
        var davidProgress = new LearnerProgress
        {
            EmployeeId = "EMP004",
            CourseId = databaseCourse.Id,
            CurrentModuleId = dbModule0?.Id,
            CurrentLessonId = dbModule0?.Lessons.FirstOrDefault()?.Id,
            ProgressPercentage = 10,
            LastAccessedAt = DateTime.UtcNow.AddHours(-12),
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        await _context.LearnerProgresses.AddAsync(davidProgress);

        // Create some assignments
        var assignment1 = new Assignment
        {
            CourseId = csharpCourse.Id,
            EmployeeId = "EMP002",
            AssignedBy = "admin",
            AssignedDate = DateTime.UtcNow.AddDays(-7),
            DueDate = DateTime.UtcNow.AddDays(7),
            Status = AssignmentStatus.InProgress,
            StartedAt = DateTime.UtcNow.AddDays(-7),
            CreatedAt = DateTime.UtcNow.AddDays(-7)
        };
        await _context.Assignments.AddAsync(assignment1);

        var assignment2 = new Assignment
        {
            CourseId = agileCourse.Id,
            EmployeeId = "EMP003",
            AssignedBy = "admin",
            AssignedDate = DateTime.UtcNow.AddDays(-5),
            DueDate = DateTime.UtcNow.AddDays(10),
            Status = AssignmentStatus.InProgress,
            StartedAt = DateTime.UtcNow.AddDays(-5),
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        };
        await _context.Assignments.AddAsync(assignment2);

        await _context.SaveChangesAsync();
    }

    private async Task CreateAssignmentsAndProgress(List<Course> courses, List<MockEmployee> employees)
    {
        var csharpCourse = courses[0];
        var agileCourse = courses[1];
        var databaseCourse = courses[2];

        // Employee 1 (Alice): Completed C# course
        var aliceAssignment1 = new CourseAssignment
        {
            UserId = "EMP001",
            CourseId = csharpCourse.Id,
            AssignedBy = "admin",
            AssignedDate = DateTime.UtcNow.AddDays(-20),
            DueDate = DateTime.UtcNow.AddDays(-5),
            Status = CourseAssignmentStatus.Completed,
            IsMandatory = true,
            Notes = "Required for software development role"
        };
        await _context.CourseAssignments.AddAsync(aliceAssignment1);

        var aliceProgress1 = new DetailedLearnerProgress
        {
            UserId = "EMP001",
            CourseId = csharpCourse.Id,
            PercentComplete = 100,
            StartedDate = DateTime.UtcNow.AddDays(-20),
            LastAccessedDate = DateTime.UtcNow.AddDays(-5),
            CompletionDate = DateTime.UtcNow.AddDays(-5),
            Score = 92,
            TimeSpentMinutes = 280,
            Attempts = 1
        };
        await _context.DetailedLearnerProgress.AddAsync(aliceProgress1);

        // Generate certificate for Alice
        var aliceCertificate = new Certificate
        {
            CertificateNumber = $"CERT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            UserId = "EMP001",
            CourseId = csharpCourse.Id,
            IssuedDate = DateTime.UtcNow.AddDays(-5),
            ExpirationDate = DateTime.UtcNow.AddDays(360),
            Score = 92,
            Status = CertificateStatus.Active
        };
        await _context.Certificates.AddAsync(aliceCertificate);

        // Employee 1 (Alice): In progress on Agile course
        var aliceAssignment2 = new CourseAssignment
        {
            UserId = "EMP001",
            CourseId = agileCourse.Id,
            AssignedBy = "admin",
            AssignedDate = DateTime.UtcNow.AddDays(-10),
            DueDate = DateTime.UtcNow.AddDays(20),
            Status = CourseAssignmentStatus.InProgress,
            IsMandatory = false
        };
        await _context.CourseAssignments.AddAsync(aliceAssignment2);

        var aliceProgress2 = new DetailedLearnerProgress
        {
            UserId = "EMP001",
            CourseId = agileCourse.Id,
            PercentComplete = 45,
            StartedDate = DateTime.UtcNow.AddDays(-8),
            LastAccessedDate = DateTime.UtcNow.AddHours(-2),
            TimeSpentMinutes = 95,
            CurrentModule = "Scrum Framework",
            Attempts = 0
        };
        await _context.DetailedLearnerProgress.AddAsync(aliceProgress2);

        // Employee 2 (Bob): In progress on C# course
        var bobAssignment1 = new CourseAssignment
        {
            UserId = "EMP002",
            CourseId = csharpCourse.Id,
            AssignedBy = "admin",
            AssignedDate = DateTime.UtcNow.AddDays(-7),
            DueDate = DateTime.UtcNow.AddDays(14),
            Status = CourseAssignmentStatus.InProgress,
            IsMandatory = true
        };
        await _context.CourseAssignments.AddAsync(bobAssignment1);

        var bobProgress1 = new DetailedLearnerProgress
        {
            UserId = "EMP002",
            CourseId = csharpCourse.Id,
            PercentComplete = 65,
            StartedDate = DateTime.UtcNow.AddDays(-7),
            LastAccessedDate = DateTime.UtcNow.AddHours(-5),
            TimeSpentMinutes = 180,
            CurrentModule = "Object-Oriented Programming",
            Attempts = 0
        };
        await _context.DetailedLearnerProgress.AddAsync(bobProgress1);

        // Employee 3 (Carol): Just started Database course
        var carolAssignment1 = new CourseAssignment
        {
            UserId = "EMP003",
            CourseId = databaseCourse.Id,
            AssignedBy = "admin",
            AssignedDate = DateTime.UtcNow.AddDays(-2),
            DueDate = DateTime.UtcNow.AddDays(30),
            Status = CourseAssignmentStatus.InProgress,
            IsMandatory = false
        };
        await _context.CourseAssignments.AddAsync(carolAssignment1);

        var carolProgress1 = new DetailedLearnerProgress
        {
            UserId = "EMP003",
            CourseId = databaseCourse.Id,
            PercentComplete = 15,
            StartedDate = DateTime.UtcNow.AddDays(-1),
            LastAccessedDate = DateTime.UtcNow.AddHours(-3),
            TimeSpentMinutes = 30,
            CurrentModule = "Introduction to Databases",
            Attempts = 0
        };
        await _context.DetailedLearnerProgress.AddAsync(carolProgress1);

        // Employee 4 (David): Assigned but not started
        var davidAssignment1 = new CourseAssignment
        {
            UserId = "EMP004",
            CourseId = agileCourse.Id,
            AssignedBy = "admin",
            AssignedDate = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(21),
            Status = CourseAssignmentStatus.Assigned,
            IsMandatory = true,
            Notes = "Required for project manager role"
        };
        await _context.CourseAssignments.AddAsync(davidAssignment1);

        // Admin user: Completed Agile course + In progress on Database course
        var adminAssignment1 = new Assignment
        {
            EmployeeId = "admin",
            CourseId = agileCourse.Id,
            AssignedBy = "system",
            AssignedDate = DateTime.UtcNow.AddDays(-15),
            DueDate = DateTime.UtcNow.AddDays(-5),
            Status = AssignmentStatus.Completed,
            StartedAt = DateTime.UtcNow.AddDays(-15),
            CompletedAt = DateTime.UtcNow.AddDays(-5)
        };
        await _context.Assignments.AddAsync(adminAssignment1);

        var adminProgress1 = new DetailedLearnerProgress
        {
            UserId = "admin",
            CourseId = agileCourse.Id,
            PercentComplete = 100,
            StartedDate = DateTime.UtcNow.AddDays(-15),
            LastAccessedDate = DateTime.UtcNow.AddDays(-5),
            CompletionDate = DateTime.UtcNow.AddDays(-5),
            Score = 95,
            TimeSpentMinutes = 200,
            Attempts = 1
        };
        await _context.DetailedLearnerProgress.AddAsync(adminProgress1);

        var adminAssignment2 = new Assignment
        {
            EmployeeId = "admin",
            CourseId = databaseCourse.Id,
            AssignedBy = "system",
            AssignedDate = DateTime.UtcNow.AddDays(-3),
            DueDate = DateTime.UtcNow.AddDays(25),
            Status = AssignmentStatus.InProgress,
            StartedAt = DateTime.UtcNow.AddDays(-2)
        };
        await _context.Assignments.AddAsync(adminAssignment2);

        var adminProgress2 = new DetailedLearnerProgress
        {
            UserId = "admin",
            CourseId = databaseCourse.Id,
            PercentComplete = 20,
            StartedDate = DateTime.UtcNow.AddDays(-2),
            LastAccessedDate = DateTime.UtcNow.AddHours(-1),
            TimeSpentMinutes = 45,
            CurrentModule = "Database Fundamentals",
            Attempts = 0
        };
        await _context.DetailedLearnerProgress.AddAsync(adminProgress2);

        await _context.SaveChangesAsync();
    }

    private async Task CreateCertificates(List<Course> courses, List<MockEmployee> employees)
    {
        var csharpCourse = courses[0];
        var agileCourse = courses[1];

        // Certificate for Sarah Johnson (C# course)
        var sarahCertificate = new Certificate
        {
            Id = Guid.NewGuid(),
            CertificateNumber = $"BAU-LMS-{DateTime.UtcNow:yyyyMMdd}-A1B2C",
            UserId = "EMP001",
            CourseId = csharpCourse.Id,
            Course = csharpCourse,
            IssuedDate = DateTime.UtcNow.AddDays(-2),
            ExpirationDate = DateTime.UtcNow.AddDays(363),
            Score = 92,
            Status = CertificateStatus.Active,
            IssuedBy = "Business As Usual LMS",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            CreatedBy = "system"
        };
        await _context.Certificates.AddAsync(sarahCertificate);

        // Certificate for Emily Rodriguez (Agile course - completed earlier)
        var emilyCertificate = new Certificate
        {
            Id = Guid.NewGuid(),
            CertificateNumber = $"BAU-LMS-{DateTime.UtcNow:yyyyMMdd}-D3E4F",
            UserId = "EMP003",
            CourseId = agileCourse.Id,
            Course = agileCourse,
            IssuedDate = DateTime.UtcNow.AddDays(-15),
            ExpirationDate = null, // No expiration
            Score = 88,
            Status = CertificateStatus.Active,
            IssuedBy = "Business As Usual LMS",
            CreatedAt = DateTime.UtcNow.AddDays(-15),
            CreatedBy = "system"
        };
        await _context.Certificates.AddAsync(emilyCertificate);

        // Certificate for Admin User (Agile course)
        var adminCertificate = new Certificate
        {
            Id = Guid.NewGuid(),
            CertificateNumber = $"BAU-LMS-{DateTime.UtcNow:yyyyMMdd}-X9Y8Z",
            UserId = "admin",
            CourseId = agileCourse.Id,
            Course = agileCourse,
            IssuedDate = DateTime.UtcNow.AddDays(-5),
            ExpirationDate = null, // No expiration
            Score = 95,
            Status = CertificateStatus.Active,
            IssuedBy = "Business As Usual LMS",
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            CreatedBy = "system"
        };
        await _context.Certificates.AddAsync(adminCertificate);

        await _context.SaveChangesAsync();
    }

    private async Task CreateNotifications(List<Course> courses, List<MockEmployee> employees)
    {
        var csharpCourse = courses[0];
        var agileCourse = courses[1];
        var databaseCourse = courses[2];

        var notifications = new List<Notification>
        {
            // Sarah - Certificate issued notification
            new Notification
            {
                Id = Guid.NewGuid(),
                EmployeeId = "EMP001",
                Type = NotificationType.CertificateIssued,
                Title = "Certificate Issued! 🎓",
                Message = $"Congratulations! You've earned a certificate for completing {csharpCourse.Title}",
                ActionUrl = "/lms/my-certificates",
                ActionText = "View Certificate",
                Priority = NotificationPriority.High,
                CourseId = csharpCourse.Id,
                IsRead = false,
                EmailSent = true,
                EmailSentAt = DateTime.UtcNow.AddDays(-2),
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },

            // Sarah - Course completed notification (older)
            new Notification
            {
                Id = Guid.NewGuid(),
                EmployeeId = "EMP001",
                Type = NotificationType.CourseCompleted,
                Title = "Course Completed! ✅",
                Message = $"You've successfully completed {csharpCourse.Title} with a score of 92%",
                ActionUrl = $"/lms/courses/{csharpCourse.Id}",
                ActionText = "View Course",
                Priority = NotificationPriority.Normal,
                CourseId = csharpCourse.Id,
                IsRead = true,
                ReadAt = DateTime.UtcNow.AddDays(-2).AddHours(1),
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },

            // Michael - Course assigned notification
            new Notification
            {
                Id = Guid.NewGuid(),
                EmployeeId = "EMP002",
                Type = NotificationType.CourseAssigned,
                Title = "New Course Assigned 📚",
                Message = $"You've been assigned: {csharpCourse.Title}. Due date: {DateTime.UtcNow.AddDays(14):MMM dd, yyyy}",
                ActionUrl = $"/lms/courses/{csharpCourse.Id}",
                ActionText = "Start Course",
                Priority = NotificationPriority.Normal,
                CourseId = csharpCourse.Id,
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            },

            // Emily - Assignment due soon
            new Notification
            {
                Id = Guid.NewGuid(),
                EmployeeId = "EMP003",
                Type = NotificationType.AssignmentDueSoon,
                Title = "Assignment Due Soon ⏰",
                Message = $"{agileCourse.Title} is due in 3 days. You're at 80% completion - almost there!",
                ActionUrl = $"/lms/courses/{agileCourse.Id}",
                ActionText = "Continue Course",
                Priority = NotificationPriority.High,
                CourseId = agileCourse.Id,
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddHours(-12)
            },

            // Emily - Certificate issued for older completion
            new Notification
            {
                Id = Guid.NewGuid(),
                EmployeeId = "EMP003",
                Type = NotificationType.CertificateIssued,
                Title = "Certificate Issued! 🎓",
                Message = $"Congratulations! You've earned a certificate for completing {agileCourse.Title}",
                ActionUrl = "/lms/my-certificates",
                ActionText = "View Certificate",
                Priority = NotificationPriority.High,
                CourseId = agileCourse.Id,
                IsRead = true,
                ReadAt = DateTime.UtcNow.AddDays(-14),
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },

            // David - New assignment (unread)
            new Notification
            {
                Id = Guid.NewGuid(),
                EmployeeId = "EMP004",
                Type = NotificationType.CourseAssigned,
                Title = "New Mandatory Training Assigned 📋",
                Message = $"You've been assigned mandatory training: {agileCourse.Title}. Please start within 7 days.",
                ActionUrl = $"/lms/courses/{agileCourse.Id}",
                ActionText = "View Assignment",
                Priority = NotificationPriority.Urgent,
                CourseId = agileCourse.Id,
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },

            // System announcement for all
            new Notification
            {
                Id = Guid.NewGuid(),
                EmployeeId = "EMP001",
                Type = NotificationType.SystemAnnouncement,
                Title = "New LMS Features Available! 🚀",
                Message = "Check out our new quiz system, certificates, and notification center!",
                ActionUrl = "/lms",
                ActionText = "Explore LMS",
                Priority = NotificationPriority.Low,
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddHours(-6)
            },

            new Notification
            {
                Id = Guid.NewGuid(),
                EmployeeId = "EMP002",
                Type = NotificationType.SystemAnnouncement,
                Title = "New LMS Features Available! 🚀",
                Message = "Check out our new quiz system, certificates, and notification center!",
                ActionUrl = "/lms",
                ActionText = "Explore LMS",
                Priority = NotificationPriority.Low,
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddHours(-6)
            },

            // Admin user notifications
            new Notification
            {
                Id = Guid.NewGuid(),
                EmployeeId = "admin",
                Type = NotificationType.CourseAssigned,
                Title = "Welcome to the LMS! 📚",
                Message = $"You've been assigned: {databaseCourse.Title}. Start learning today!",
                ActionUrl = $"/lms/courses/{databaseCourse.Id}",
                ActionText = "Start Course",
                Priority = NotificationPriority.High,
                CourseId = databaseCourse.Id,
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            },

            new Notification
            {
                Id = Guid.NewGuid(),
                EmployeeId = "admin",
                Type = NotificationType.CertificateIssued,
                Title = "Certificate Issued! 🎓",
                Message = $"Congratulations! You've earned a certificate for completing {agileCourse.Title}",
                ActionUrl = "/lms/my-certificates",
                ActionText = "View Certificate",
                Priority = NotificationPriority.High,
                CourseId = agileCourse.Id,
                IsRead = false,
                EmailSent = true,
                EmailSentAt = DateTime.UtcNow.AddDays(-5),
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },

            new Notification
            {
                Id = Guid.NewGuid(),
                EmployeeId = "admin",
                Type = NotificationType.SystemAnnouncement,
                Title = "New LMS Features Available! 🚀",
                Message = "Check out our new quiz system, certificates, and notification center!",
                ActionUrl = "/lms",
                ActionText = "Explore LMS",
                Priority = NotificationPriority.Low,
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddHours(-6)
            }
        };

        await _context.Notifications.AddRangeAsync(notifications);
        await _context.SaveChangesAsync();
    }

    private class MockEmployee
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
    }
}
