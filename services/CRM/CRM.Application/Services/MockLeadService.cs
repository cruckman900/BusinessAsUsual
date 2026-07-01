using CRM.Application.DTOs;
using CRM.Domain.Entities;
using CRM.Domain.Enums;

namespace CRM.Application.Services;

public class MockLeadService : ILeadService
{
    private static readonly List<Lead> _leads = GenerateSampleLeads();

    public Task<IEnumerable<LeadDto>> GetAllLeadsAsync()
    {
        var dtos = _leads.Select(MapToDto);
        return Task.FromResult(dtos);
    }

    public Task<LeadDto?> GetLeadByIdAsync(string id)
    {
        var lead = _leads.FirstOrDefault(l => l.Id == id);
        return Task.FromResult(lead != null ? MapToDto(lead) : null);
    }

    public Task<LeadDto> CreateLeadAsync(CreateLeadRequest request)
    {
        var lead = new Lead
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Company = request.Company,
            JobTitle = request.JobTitle,
            Source = request.Source,
            EstimatedValue = request.EstimatedValue,
            Description = request.Description,
            AssignedToEmployeeId = request.AssignedToEmployeeId,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country,
            LinkedInProfile = request.LinkedInProfile,
            TwitterHandle = request.TwitterHandle,
            Tags = request.Tags,
            Status = LeadStatus.New,
            CreatedDate = DateTime.UtcNow
        };
        _leads.Add(lead);
        return Task.FromResult(MapToDto(lead));
    }

    public Task<LeadDto> UpdateLeadAsync(string id, UpdateLeadRequest request)
    {
        var lead = _leads.FirstOrDefault(l => l.Id == id);
        if (lead == null) throw new KeyNotFoundException($"Lead {id} not found");

        lead.FirstName = request.FirstName;
        lead.LastName = request.LastName;
        lead.Email = request.Email;
        lead.Phone = request.Phone;
        lead.Company = request.Company;
        lead.JobTitle = request.JobTitle;
        lead.Status = request.Status;
        lead.Source = request.Source;
        lead.EstimatedValue = request.EstimatedValue;
        lead.Description = request.Description;
        lead.AssignedToEmployeeId = request.AssignedToEmployeeId;
        lead.AddressLine1 = request.AddressLine1;
        lead.AddressLine2 = request.AddressLine2;
        lead.City = request.City;
        lead.State = request.State;
        lead.PostalCode = request.PostalCode;
        lead.Country = request.Country;
        lead.LinkedInProfile = request.LinkedInProfile;
        lead.TwitterHandle = request.TwitterHandle;
        lead.Tags = request.Tags;

        return Task.FromResult(MapToDto(lead));
    }

    public Task DeleteLeadAsync(string id)
    {
        var lead = _leads.FirstOrDefault(l => l.Id == id);
        if (lead != null) _leads.Remove(lead);
        return Task.CompletedTask;
    }

    public Task<LeadDto> ConvertLeadToCustomerAsync(string id)
    {
        var lead = _leads.FirstOrDefault(l => l.Id == id);
        if (lead == null) throw new KeyNotFoundException($"Lead {id} not found");

        lead.Status = LeadStatus.Converted;
        lead.ConvertedDate = DateTime.UtcNow;
        lead.ConvertedToCustomerId = Guid.NewGuid().ToString();

        return Task.FromResult(MapToDto(lead));
    }

    private static LeadDto MapToDto(Lead lead)
    {
        return new LeadDto
        {
            Id = lead.Id,
            FirstName = lead.FirstName,
            LastName = lead.LastName,
            FullName = lead.FullName,
            Email = lead.Email,
            Phone = lead.Phone,
            Company = lead.Company,
            JobTitle = lead.JobTitle,
            Status = lead.Status.ToString(),
            Source = lead.Source.ToString(),
            EstimatedValue = lead.EstimatedValue,
            Description = lead.Description,
            AssignedToEmployeeId = lead.AssignedToEmployeeId,
            AddressLine1 = lead.AddressLine1,
            AddressLine2 = lead.AddressLine2,
            City = lead.City,
            State = lead.State,
            PostalCode = lead.PostalCode,
            Country = lead.Country,
            LinkedInProfile = lead.LinkedInProfile,
            TwitterHandle = lead.TwitterHandle,
            CreatedDate = lead.CreatedDate,
            LastContactedDate = lead.LastContactedDate,
            QualifiedDate = lead.QualifiedDate,
            ConvertedDate = lead.ConvertedDate,
            ConvertedToCustomerId = lead.ConvertedToCustomerId,
            Tags = lead.Tags,
            ActivityCount = lead.Activities?.Count ?? 0
        };
    }

    private static List<Lead> GenerateSampleLeads()
    {
        var now = DateTime.UtcNow;
        return new List<Lead>
        {
            new() { Id = "1", FirstName = "John", LastName = "Smith", Email = "john.smith@techcorp.com", Company = "TechCorp", JobTitle = "CTO", Status = LeadStatus.Qualified, Source = LeadSource.Website, EstimatedValue = 50000, Phone = "555-0101", Tags = new List<string> { "Enterprise", "Tech" }, CreatedDate = now.AddDays(-45) },
            new() { Id = "2", FirstName = "Sarah", LastName = "Johnson", Email = "sarah.j@innovate.com", Company = "Innovate Inc", JobTitle = "VP Sales", Status = LeadStatus.Contacted, Source = LeadSource.Referral, EstimatedValue = 75000, Phone = "555-0102", Tags = new List<string> { "Referral", "Hot" }, CreatedDate = now.AddDays(-30) },
            new() { Id = "3", FirstName = "Michael", LastName = "Chen", Email = "m.chen@startupco.io", Company = "StartupCo", JobTitle = "Founder", Status = LeadStatus.New, Source = LeadSource.SocialMedia, EstimatedValue = 25000, Phone = "555-0103", Tags = new List<string> { "Startup" }, CreatedDate = now.AddDays(-15) },
            new() { Id = "4", FirstName = "Emily", LastName = "Davis", Email = "emily.d@enterprise.com", Company = "Enterprise Solutions", JobTitle = "Director", Status = LeadStatus.Qualified, Source = LeadSource.EmailCampaign, EstimatedValue = 100000, Phone = "555-0104", Tags = new List<string> { "Enterprise", "Priority" }, CreatedDate = now.AddDays(-60) },
            new() { Id = "5", FirstName = "David", LastName = "Wilson", Email = "dwilson@consulting.com", Company = "Wilson Consulting", JobTitle = "Managing Partner", Status = LeadStatus.Contacted, Source = LeadSource.Event, EstimatedValue = 40000, Phone = "555-0105", Tags = new List<string> { "Consulting" }, CreatedDate = now.AddDays(-20) },
            new() { Id = "6", FirstName = "Lisa", LastName = "Anderson", Email = "l.anderson@bigcorp.com", Company = "BigCorp Industries", JobTitle = "Procurement Manager", Status = LeadStatus.Qualified, Source = LeadSource.Website, EstimatedValue = 120000, Phone = "555-0106", Tags = new List<string> { "Enterprise" }, CreatedDate = now.AddDays(-10) },
            new() { Id = "7", FirstName = "Robert", LastName = "Martinez", Email = "r.martinez@techstart.io", Company = "TechStart", JobTitle = "CEO", Status = LeadStatus.Converted, Source = LeadSource.Referral, EstimatedValue = 65000, Phone = "555-0107", Tags = new List<string> { "Converted" }, CreatedDate = now.AddDays(-90), ConvertedDate = now.AddDays(-30) },
            new() { Id = "8", FirstName = "Jennifer", LastName = "Lee", Email = "j.lee@growthco.com", Company = "GrowthCo", JobTitle = "COO", Status = LeadStatus.New, Source = LeadSource.SocialMedia, EstimatedValue = 35000, Phone = "555-0108", Tags = new List<string> { "SaaS" }, CreatedDate = now.AddDays(-5) }
        };
    }
}
