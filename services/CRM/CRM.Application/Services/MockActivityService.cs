using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Domain.Enums;

namespace CRM.Application.Services;

public class MockActivityService : IActivityService
{
    private readonly List<Activity> _activities;

    public MockActivityService()
    {
        _activities = GenerateSampleActivities();
    }

    public Task<IEnumerable<ActivityDto>> GetAllActivitiesAsync()
    {
        return Task.FromResult(_activities.Select(MapToDto));
    }

    public Task<ActivityDto?> GetActivityByIdAsync(string id)
    {
        var activity = _activities.FirstOrDefault(a => a.Id == id);
        return Task.FromResult(activity != null ? MapToDto(activity) : null);
    }

    public Task<IEnumerable<ActivityDto>> GetActivitiesByLeadIdAsync(string leadId)
    {
        var activities = _activities.Where(a => a.LeadId == leadId).Select(MapToDto);
        return Task.FromResult(activities);
    }

    public Task<IEnumerable<ActivityDto>> GetActivitiesByOpportunityIdAsync(string opportunityId)
    {
        var activities = _activities.Where(a => a.OpportunityId == opportunityId).Select(MapToDto);
        return Task.FromResult(activities);
    }

    public Task<IEnumerable<ActivityDto>> GetActivitiesByCustomerIdAsync(string customerId)
    {
        var activities = _activities.Where(a => a.CustomerId == customerId).Select(MapToDto);
        return Task.FromResult(activities);
    }

    public Task<IEnumerable<ActivityDto>> GetUpcomingActivitiesAsync(int days = 7)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(days);
        var activities = _activities
            .Where(a => !a.IsCompleted && a.DueDate.HasValue && a.DueDate.Value <= cutoffDate)
            .OrderBy(a => a.DueDate)
            .Select(MapToDto);
        return Task.FromResult(activities);
    }

    public Task<IEnumerable<ActivityDto>> GetOverdueActivitiesAsync()
    {
        var now = DateTime.UtcNow;
        var activities = _activities
            .Where(a => !a.IsCompleted && a.DueDate.HasValue && a.DueDate.Value < now)
            .OrderBy(a => a.DueDate)
            .Select(MapToDto);
        return Task.FromResult(activities);
    }

    public Task<ActivityDto> CreateActivityAsync(CreateActivityRequest request)
    {
        var activity = new Activity
        {
            Id = Guid.NewGuid().ToString(),
            Type = request.Type,
            Subject = request.Subject,
            Description = request.Description,
            ActivityDate = request.ActivityDate,
            DueDate = request.DueDate,
            Priority = request.Priority,
            DurationMinutes = request.DurationMinutes,
            LeadId = request.LeadId,
            OpportunityId = request.OpportunityId,
            CustomerId = request.CustomerId,
            AssignedToEmployeeId = request.AssignedToEmployeeId,
            CreatedDate = DateTime.UtcNow
        };

        _activities.Add(activity);
        return Task.FromResult(MapToDto(activity));
    }

    public Task<ActivityDto> UpdateActivityAsync(string id, UpdateActivityRequest request)
    {
        var activity = _activities.FirstOrDefault(a => a.Id == id);
        if (activity == null)
            throw new InvalidOperationException($"Activity with ID {id} not found");

        activity.Type = request.Type;
        activity.Subject = request.Subject;
        activity.Description = request.Description;
        activity.ActivityDate = request.ActivityDate;
        activity.DueDate = request.DueDate;
        activity.IsCompleted = request.IsCompleted;
        activity.CompletedDate = request.CompletedDate;
        activity.Priority = request.Priority;
        activity.DurationMinutes = request.DurationMinutes;
        activity.AssignedToEmployeeId = request.AssignedToEmployeeId;
        activity.Outcome = request.Outcome;
        activity.NextSteps = request.NextSteps;

        return Task.FromResult(MapToDto(activity));
    }

    public Task DeleteActivityAsync(string id)
    {
        var activity = _activities.FirstOrDefault(a => a.Id == id);
        if (activity != null)
            _activities.Remove(activity);

        return Task.CompletedTask;
    }

    public Task<ActivityDto> CompleteActivityAsync(string id, string? outcome = null, string? nextSteps = null)
    {
        var activity = _activities.FirstOrDefault(a => a.Id == id);
        if (activity == null)
            throw new InvalidOperationException($"Activity with ID {id} not found");

        activity.IsCompleted = true;
        activity.CompletedDate = DateTime.UtcNow;
        if (outcome != null) activity.Outcome = outcome;
        if (nextSteps != null) activity.NextSteps = nextSteps;

        return Task.FromResult(MapToDto(activity));
    }

    private List<Activity> GenerateSampleActivities()
    {
        var now = DateTime.UtcNow;
        return new List<Activity>
        {
            // Past completed activities
            new Activity
            {
                Id = "ACT-001",
                Type = ActivityType.Call,
                Subject = "Initial discovery call with Tech Innovations",
                Description = "Discussed their project management needs and current pain points",
                ActivityDate = now.AddDays(-15),
                DueDate = now.AddDays(-15),
                IsCompleted = true,
                CompletedDate = now.AddDays(-15),
                Priority = Priority.High,
                DurationMinutes = 30,
                LeadId = "LEAD-001",
                AssignedToEmployeeId = "EMP-001",
                Outcome = "Qualified lead, moving to opportunity stage",
                NextSteps = "Schedule product demo",
                CreatedDate = now.AddDays(-16)
            },
            new Activity
            {
                Id = "ACT-002",
                Type = ActivityType.Meeting,
                Subject = "Product demonstration for Enterprise Solutions Corp",
                Description = "Full platform walkthrough with decision makers",
                ActivityDate = now.AddDays(-10),
                DueDate = now.AddDays(-10),
                IsCompleted = true,
                CompletedDate = now.AddDays(-10),
                Priority = Priority.High,
                DurationMinutes = 60,
                OpportunityId = "OPP-001",
                AssignedToEmployeeId = "EMP-001",
                Outcome = "Very positive response, requested proposal",
                NextSteps = "Send formal proposal by end of week",
                CreatedDate = now.AddDays(-12)
            },
            new Activity
            {
                Id = "ACT-003",
                Type = ActivityType.Email,
                Subject = "Follow-up proposal for Enterprise Solutions Corp",
                Description = "Sent detailed proposal with pricing and implementation timeline",
                ActivityDate = now.AddDays(-8),
                DueDate = now.AddDays(-8),
                IsCompleted = true,
                CompletedDate = now.AddDays(-8),
                Priority = Priority.High,
                DurationMinutes = 45,
                OpportunityId = "OPP-001",
                AssignedToEmployeeId = "EMP-001",
                Outcome = "Proposal delivered on time",
                NextSteps = "Schedule follow-up call in 3 days",
                CreatedDate = now.AddDays(-9)
            },
            new Activity
            {
                Id = "ACT-004",
                Type = ActivityType.Call,
                Subject = "Check in with Digital Marketing Hub",
                Description = "Quarterly business review call",
                ActivityDate = now.AddDays(-5),
                DueDate = now.AddDays(-5),
                IsCompleted = true,
                CompletedDate = now.AddDays(-5),
                Priority = Priority.Medium,
                DurationMinutes = 30,
                CustomerId = "CUST-001",
                AssignedToEmployeeId = "EMP-002",
                Outcome = "Customer satisfied, discussed expansion",
                NextSteps = "Send expansion proposal next quarter",
                CreatedDate = now.AddDays(-7)
            },

            // Recent activities
            new Activity
            {
                Id = "ACT-005",
                Type = ActivityType.Meeting,
                Subject = "Contract negotiation with Enterprise Solutions Corp",
                Description = "Review terms, pricing adjustments, and implementation schedule",
                ActivityDate = now.AddDays(-2),
                DueDate = now.AddDays(-2),
                IsCompleted = true,
                CompletedDate = now.AddDays(-2),
                Priority = Priority.High,
                DurationMinutes = 90,
                OpportunityId = "OPP-001",
                AssignedToEmployeeId = "EMP-001",
                Outcome = "Agreement reached on terms",
                NextSteps = "Legal review and signature",
                CreatedDate = now.AddDays(-3)
            },

            // Today's activities
            new Activity
            {
                Id = "ACT-006",
                Type = ActivityType.Call,
                Subject = "Follow-up call with Retail Chain Solutions",
                Description = "Discuss demo feedback and next steps",
                ActivityDate = now,
                DueDate = now,
                IsCompleted = false,
                Priority = Priority.High,
                DurationMinutes = 30,
                OpportunityId = "OPP-004",
                AssignedToEmployeeId = "EMP-001",
                CreatedDate = now.AddDays(-1)
            },
            new Activity
            {
                Id = "ACT-007",
                Type = ActivityType.Email,
                Subject = "Send case studies to Healthcare Systems Inc",
                Description = "Share relevant customer success stories in healthcare vertical",
                ActivityDate = now,
                DueDate = now,
                IsCompleted = false,
                Priority = Priority.Medium,
                DurationMinutes = 20,
                LeadId = "LEAD-004",
                AssignedToEmployeeId = "EMP-002",
                CreatedDate = now.AddDays(-1)
            },

            // Upcoming activities
            new Activity
            {
                Id = "ACT-008",
                Type = ActivityType.Meeting,
                Subject = "Discovery meeting with Cloud Services Provider",
                Description = "Initial needs assessment and platform overview",
                ActivityDate = now.AddDays(2),
                DueDate = now.AddDays(2),
                IsCompleted = false,
                Priority = Priority.High,
                DurationMinutes = 60,
                LeadId = "LEAD-006",
                AssignedToEmployeeId = "EMP-001",
                CreatedDate = now.AddDays(-2)
            },
            new Activity
            {
                Id = "ACT-009",
                Type = ActivityType.Call,
                Subject = "Proposal review with Manufacturing Automation Corp",
                Description = "Walk through proposal details and answer questions",
                ActivityDate = now.AddDays(3),
                DueDate = now.AddDays(3),
                IsCompleted = false,
                Priority = Priority.High,
                DurationMinutes = 45,
                OpportunityId = "OPP-005",
                AssignedToEmployeeId = "EMP-001",
                CreatedDate = now.AddDays(-1)
            },
            new Activity
            {
                Id = "ACT-010",
                Type = ActivityType.Task,
                Subject = "Prepare custom demo for Financial Services Group",
                Description = "Build demo environment with financial industry customizations",
                ActivityDate = now.AddDays(4),
                DueDate = now.AddDays(4),
                IsCompleted = false,
                Priority = Priority.Medium,
                DurationMinutes = 120,
                OpportunityId = "OPP-003",
                AssignedToEmployeeId = "EMP-002",
                CreatedDate = now
            },
            new Activity
            {
                Id = "ACT-011",
                Type = ActivityType.Meeting,
                Subject = "Quarterly business review with Tech Innovations",
                Description = "Review usage, ROI, and discuss additional modules",
                ActivityDate = now.AddDays(7),
                DueDate = now.AddDays(7),
                IsCompleted = false,
                Priority = Priority.Medium,
                DurationMinutes = 60,
                CustomerId = "CUST-005",
                AssignedToEmployeeId = "EMP-001",
                CreatedDate = now.AddDays(-1)
            },

            // Overdue activities
            new Activity
            {
                Id = "ACT-012",
                Type = ActivityType.Call,
                Subject = "Follow-up with E-commerce Platform Startup",
                Description = "No response to initial demo invitation",
                ActivityDate = now.AddDays(-3),
                DueDate = now.AddDays(-3),
                IsCompleted = false,
                Priority = Priority.Low,
                DurationMinutes = 15,
                LeadId = "LEAD-008",
                AssignedToEmployeeId = "EMP-002",
                CreatedDate = now.AddDays(-5)
            },
            new Activity
            {
                Id = "ACT-013",
                Type = ActivityType.Email,
                Subject = "Send contract renewal to Smart Analytics Co",
                Description = "Annual renewal package with updated pricing",
                ActivityDate = now.AddDays(-1),
                DueDate = now.AddDays(-1),
                IsCompleted = false,
                Priority = Priority.High,
                DurationMinutes = 30,
                CustomerId = "CUST-004",
                AssignedToEmployeeId = "EMP-001",
                CreatedDate = now.AddDays(-3)
            }
        };
    }

    private ActivityDto MapToDto(Activity activity)
    {
        return new ActivityDto
        {
            Id = activity.Id,
            Type = activity.Type.ToString(),
            Subject = activity.Subject,
            Description = activity.Description,
            ActivityDate = activity.ActivityDate,
            DueDate = activity.DueDate,
            IsCompleted = activity.IsCompleted,
            CompletedDate = activity.CompletedDate,
            Priority = activity.Priority.ToString(),
            DurationMinutes = activity.DurationMinutes,
            LeadId = activity.LeadId,
            OpportunityId = activity.OpportunityId,
            CustomerId = activity.CustomerId,
            AssignedToEmployeeId = activity.AssignedToEmployeeId,
            AssignedToEmployeeName = activity.AssignedToEmployeeId != null ? $"Employee {activity.AssignedToEmployeeId}" : null,
            Outcome = activity.Outcome,
            NextSteps = activity.NextSteps,
            CreatedDate = activity.CreatedDate
        };
    }
}
