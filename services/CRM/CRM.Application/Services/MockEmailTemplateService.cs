using System.Text.RegularExpressions;
using CRM.Application.DTOs;
using CRM.Application.Interfaces;

namespace CRM.Application.Services;

/// <summary>
/// In-memory email template store with a library of seed templates across common
/// sales categories. Mirrors the mock-service pattern used elsewhere in the CRM.
/// </summary>
public class MockEmailTemplateService : IEmailTemplateService
{
    private static readonly Regex MergeFieldPattern = new(@"\{\{\s*(\w+)\s*\}\}", RegexOptions.Compiled);

    private readonly List<EmailTemplateDto> _templates;

    public MockEmailTemplateService()
    {
        var now = DateTime.UtcNow;
        _templates = new List<EmailTemplateDto>
        {
            new()
            {
                Id = "T1",
                Name = "Initial Follow-up",
                Category = "Follow-up",
                Subject = "Great connecting, {{ContactName}}",
                Body = "Hi {{ContactName}},\n\nThank you for taking the time to chat today. I'm excited about the opportunity to help {{CompanyName}} reach its goals.\n\nI'll follow up shortly with next steps. In the meantime, feel free to reach out with any questions.\n\nBest regards,\n{{SenderName}}",
                IsActive = true,
                CreatedDate = now.AddDays(-30),
            },
            new()
            {
                Id = "T2",
                Name = "Proposal Delivery",
                Category = "Proposal",
                Subject = "Your proposal for {{OpportunityName}}",
                Body = "Hi {{ContactName}},\n\nPlease find attached our proposal for {{OpportunityName}}, valued at {{Amount}}.\n\nWe've tailored this to the priorities we discussed with {{CompanyName}}. I'd love to walk you through it — are you available this week?\n\nWarm regards,\n{{SenderName}}",
                IsActive = true,
                CreatedDate = now.AddDays(-25),
            },
            new()
            {
                Id = "T3",
                Name = "Check-in / Nurture",
                Category = "Follow-up",
                Subject = "Checking in, {{ContactName}}",
                Body = "Hi {{ContactName}},\n\nI wanted to check in and see how things are going at {{CompanyName}}. Is there anything I can help with as you evaluate your options?\n\nHappy to jump on a quick call whenever it's convenient.\n\nBest,\n{{SenderName}}",
                IsActive = true,
                CreatedDate = now.AddDays(-20),
            },
            new()
            {
                Id = "T4",
                Name = "Meeting Request",
                Category = "Scheduling",
                Subject = "Time to connect about {{OpportunityName}}?",
                Body = "Hi {{ContactName}},\n\nI'd like to schedule 30 minutes to discuss {{OpportunityName}} and how we can support {{CompanyName}}.\n\nWould any of these times work for you? I'm flexible to fit your schedule.\n\nThanks,\n{{SenderName}}",
                IsActive = true,
                CreatedDate = now.AddDays(-15),
            },
            new()
            {
                Id = "T5",
                Name = "Thank You / Closed Won",
                Category = "Closing",
                Subject = "Welcome aboard, {{ContactName}}!",
                Body = "Hi {{ContactName}},\n\nThank you for choosing us! We're thrilled to partner with {{CompanyName}} and can't wait to get started.\n\nYour dedicated team will reach out shortly to kick things off.\n\nWelcome aboard,\n{{SenderName}}",
                IsActive = true,
                CreatedDate = now.AddDays(-10),
            },
        };

        foreach (var t in _templates)
        {
            t.MergeFields = ExtractMergeFields(t.Subject, t.Body);
        }
    }

    public Task<IEnumerable<EmailTemplateDto>> GetAllTemplatesAsync()
        => Task.FromResult<IEnumerable<EmailTemplateDto>>(_templates.OrderBy(t => t.Category).ThenBy(t => t.Name).ToList());

    public Task<IEnumerable<EmailTemplateDto>> GetTemplatesByCategoryAsync(string category)
        => Task.FromResult<IEnumerable<EmailTemplateDto>>(
            _templates.Where(t => string.Equals(t.Category, category, StringComparison.OrdinalIgnoreCase)).ToList());

    public Task<EmailTemplateDto?> GetTemplateByIdAsync(string id)
        => Task.FromResult(_templates.FirstOrDefault(t => t.Id == id));

    public Task<IEnumerable<string>> GetCategoriesAsync()
        => Task.FromResult<IEnumerable<string>>(_templates.Select(t => t.Category).Distinct().OrderBy(c => c).ToList());

    public Task<EmailTemplateDto> CreateTemplateAsync(CreateEmailTemplateRequest request)
    {
        var template = new EmailTemplateDto
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            Name = request.Name,
            Category = request.Category,
            Subject = request.Subject,
            Body = request.Body,
            IsActive = request.IsActive,
            CreatedDate = DateTime.UtcNow,
            MergeFields = ExtractMergeFields(request.Subject, request.Body),
        };
        _templates.Add(template);
        return Task.FromResult(template);
    }

    public Task<EmailTemplateDto> UpdateTemplateAsync(string id, UpdateEmailTemplateRequest request)
    {
        var template = _templates.FirstOrDefault(t => t.Id == id)
            ?? throw new KeyNotFoundException($"Email template {id} not found");

        template.Name = request.Name;
        template.Category = request.Category;
        template.Subject = request.Subject;
        template.Body = request.Body;
        template.IsActive = request.IsActive;
        template.LastModifiedDate = DateTime.UtcNow;
        template.MergeFields = ExtractMergeFields(request.Subject, request.Body);

        return Task.FromResult(template);
    }

    public Task DeleteTemplateAsync(string id)
    {
        _templates.RemoveAll(t => t.Id == id);
        return Task.CompletedTask;
    }

    public string ApplyMergeFields(string text, IReadOnlyDictionary<string, string?> values)
    {
        if (string.IsNullOrEmpty(text)) return text;

        return MergeFieldPattern.Replace(text, match =>
        {
            var token = match.Groups[1].Value;
            return values.TryGetValue(token, out var value) && value is not null
                ? value
                : match.Value;
        });
    }

    private static List<string> ExtractMergeFields(params string[] sources)
    {
        var fields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var source in sources)
        {
            if (string.IsNullOrEmpty(source)) continue;
            foreach (Match match in MergeFieldPattern.Matches(source))
            {
                fields.Add(match.Groups[1].Value);
            }
        }
        return fields.OrderBy(f => f).ToList();
    }
}
