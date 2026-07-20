using CRM.Application.DTOs;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Core.Events.Integration;

namespace CRM.Application.Services;

public class MockOpportunityService : IOpportunityService
{
    private static readonly List<Opportunity> _opportunities = GenerateSampleOpportunities();

    private readonly IEventBus? _eventBus;

    // IEventBus is optional so hosts that don't wire the bus (e.g. CRM.Web)
    // keep working; when present (CRM.API), won opportunities are published.
    public MockOpportunityService(IEventBus? eventBus = null)
    {
        _eventBus = eventBus;
    }

    public Task<IEnumerable<OpportunityDto>> GetAllOpportunitiesAsync()
    {
        var dtos = _opportunities.Select(MapToDto);
        return Task.FromResult(dtos);
    }

    public Task<OpportunityDto?> GetOpportunityByIdAsync(string id)
    {
        var opp = _opportunities.FirstOrDefault(o => o.Id == id);
        return Task.FromResult(opp != null ? MapToDto(opp) : null);
    }

    public Task<IEnumerable<OpportunityDto>> GetOpportunitiesByCustomerAsync(string customerId)
    {
        var dtos = _opportunities.Where(o => o.CustomerId == customerId).Select(MapToDto);
        return Task.FromResult(dtos);
    }

    public Task<OpportunityDto> CreateOpportunityAsync(CreateOpportunityRequest request)
    {
        var opp = new Opportunity
        {
            Name = request.Name,
            CustomerId = request.CustomerId,
            LeadId = request.LeadId,
            Stage = request.Stage,
            Amount = request.Amount,
            Probability = request.Probability,
            ExpectedCloseDate = request.ExpectedCloseDate,
            Description = request.Description,
            AssignedToEmployeeId = request.AssignedToEmployeeId,
            ProductCategory = request.ProductCategory,
            ProductDescription = request.ProductDescription,
            Quantity = request.Quantity,
            Tags = request.Tags,
            CreatedDate = DateTime.UtcNow
        };
        _opportunities.Add(opp);
        return Task.FromResult(MapToDto(opp));
    }

    public Task<OpportunityDto> UpdateOpportunityAsync(string id, UpdateOpportunityRequest request)
    {
        var opp = _opportunities.FirstOrDefault(o => o.Id == id);
        if (opp == null) throw new KeyNotFoundException($"Opportunity {id} not found");

        var wasWon = opp.IsWon;

        opp.Name = request.Name;
        opp.CustomerId = request.CustomerId;
        opp.Stage = request.Stage;
        opp.Amount = request.Amount;
        opp.Probability = request.Probability;
        opp.ExpectedCloseDate = request.ExpectedCloseDate;
        opp.Description = request.Description;
        opp.AssignedToEmployeeId = request.AssignedToEmployeeId;
        opp.LostReason = request.LostReason;
        opp.CompetitorName = request.CompetitorName;
        opp.ProductCategory = request.ProductCategory;
        opp.ProductDescription = request.ProductDescription;
        opp.Quantity = request.Quantity;
        opp.Tags = request.Tags;

        if (opp.IsClosed && opp.ActualCloseDate == null)
        {
            opp.ActualCloseDate = DateTime.UtcNow;
        }

        // Publish an integration event on the won transition so downstream
        // modules (e.g. Finance) can react. Fire-and-forget via the bus.
        if (!wasWon && opp.IsWon && _eventBus is not null)
        {
            _ = _eventBus.PublishAsync(new OpportunityWonIntegrationEvent
            {
                OpportunityId = opp.Id,
                OpportunityName = opp.Name,
                CustomerId = opp.CustomerId,
                CustomerName = opp.CustomerId ?? opp.Name,
                Amount = opp.Amount,
                ProductCategory = opp.ProductCategory,
                ProductDescription = opp.ProductDescription,
                Quantity = opp.Quantity,
                AssignedToEmployeeId = opp.AssignedToEmployeeId
            });
        }

        return Task.FromResult(MapToDto(opp));
    }

    public Task DeleteOpportunityAsync(string id)
    {
        var opp = _opportunities.FirstOrDefault(o => o.Id == id);
        if (opp != null) _opportunities.Remove(opp);
        return Task.CompletedTask;
    }

    private static OpportunityDto MapToDto(Opportunity opp)
    {
        return new OpportunityDto
        {
            Id = opp.Id,
            Name = opp.Name,
            CustomerId = opp.CustomerId,
            LeadId = opp.LeadId,
            Stage = opp.Stage.ToString(),
            Amount = opp.Amount,
            Probability = opp.Probability,
            ExpectedCloseDate = opp.ExpectedCloseDate,
            ActualCloseDate = opp.ActualCloseDate,
            Description = opp.Description,
            AssignedToEmployeeId = opp.AssignedToEmployeeId,
            IsWon = opp.IsWon,
            IsLost = opp.IsLost,
            LostReason = opp.LostReason,
            CompetitorName = opp.CompetitorName,
            ProductCategory = opp.ProductCategory,
            ProductDescription = opp.ProductDescription,
            Quantity = opp.Quantity,
            CreatedDate = opp.CreatedDate,
            LastActivityDate = opp.LastActivityDate,
            DaysInStage = opp.DaysInStage,
            Tags = opp.Tags,
            ActivityCount = opp.Activities?.Count ?? 0
        };
    }

    private static List<Opportunity> GenerateSampleOpportunities()
    {
        var now = DateTime.UtcNow;
        return new List<Opportunity>
        {
            new() { Id = "1", Name = "TechCorp Enterprise License", CustomerId = "C1", Stage = OpportunityStage.Proposal, Amount = 150000, Probability = 75, ExpectedCloseDate = now.AddDays(15), Tags = new List<string> { "Enterprise", "Software" }, CreatedDate = now.AddDays(-40) },
            new() { Id = "2", Name = "Innovate Inc Consulting", CustomerId = "C2", Stage = OpportunityStage.Negotiation, Amount = 85000, Probability = 80, ExpectedCloseDate = now.AddDays(7), Tags = new List<string> { "Consulting", "High-Value" }, CreatedDate = now.AddDays(-25) },
            new() { Id = "3", Name = "StartupCo Implementation", CustomerId = "C3", Stage = OpportunityStage.Qualification, Amount = 35000, Probability = 40, ExpectedCloseDate = now.AddDays(30), Tags = new List<string> { "Startup" }, CreatedDate = now.AddDays(-12) },
            new() { Id = "4", Name = "Enterprise Solutions Platform", CustomerId = "C4", Stage = OpportunityStage.ClosedWon, Amount = 250000, Probability = 100, ExpectedCloseDate = now.AddDays(-5), ActualCloseDate = now.AddDays(-5), Tags = new List<string> { "Won", "Large Deal" }, CreatedDate = now.AddDays(-60) },
            new() { Id = "5", Name = "Wilson Consulting Services", CustomerId = "C5", Stage = OpportunityStage.Prospecting, Amount = 45000, Probability = 25, ExpectedCloseDate = now.AddDays(45), Tags = new List<string> { "Consulting" }, CreatedDate = now.AddDays(-8) },
            new() { Id = "6", Name = "BigCorp Cloud Migration", CustomerId = "C1", Stage = OpportunityStage.Proposal, Amount = 180000, Probability = 70, ExpectedCloseDate = now.AddDays(20), Tags = new List<string> { "Enterprise", "Cloud" }, CreatedDate = now.AddDays(-35) },
            new() { Id = "7", Name = "TechStart MVP Development", CustomerId = "C3", Stage = OpportunityStage.Qualification, Amount = 55000, Probability = 50, ExpectedCloseDate = now.AddDays(25), Tags = new List<string> { "Development" }, CreatedDate = now.AddDays(-18) },
            new() { Id = "8", Name = "GrowthCo Marketing Platform", CustomerId = "C6", Stage = OpportunityStage.ClosedLost, Amount = 95000, Probability = 0, ExpectedCloseDate = now.AddDays(-15), ActualCloseDate = now.AddDays(-15), LostReason = "Budget constraints", Tags = new List<string> { "Lost" }, CreatedDate = now.AddDays(-75) },
            new() { Id = "9", Name = "Innovate Inc Phase 2", CustomerId = "C2", Stage = OpportunityStage.Negotiation, Amount = 125000, Probability = 85, ExpectedCloseDate = now.AddDays(10), Tags = new List<string> { "Consulting", "Expansion" }, CreatedDate = now.AddDays(-22) },
            new() { Id = "10", Name = "SmallBiz Support Package", CustomerId = "C7", Stage = OpportunityStage.Prospecting, Amount = 15000, Probability = 30, ExpectedCloseDate = now.AddDays(50), Tags = new List<string> { "SMB" }, CreatedDate = now.AddDays(-5) }
        };
    }
}

public class MockCustomerService : ICustomerService
{
    private static readonly List<Customer> _customers = GenerateSampleCustomers();

    public Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        var dtos = _customers.Select(MapToDto);
        return Task.FromResult(dtos);
    }

    public Task<CustomerDto?> GetCustomerByIdAsync(string id)
    {
        var customer = _customers.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(customer != null ? MapToDto(customer) : null);
    }

    public Task<CustomerDto> CreateCustomerAsync(CreateCustomerRequest request)
    {
        var customer = new Customer
        {
            Name = request.Name,
            CompanyName = request.CompanyName,
            Email = request.Email,
            Phone = request.Phone,
            Website = request.Website,
            Industry = request.Industry,
            EmployeeCount = request.EmployeeCount,
            AnnualRevenue = request.AnnualRevenue,
            Description = request.Description,
            PrimaryContactName = request.PrimaryContactName,
            PrimaryContactEmail = request.PrimaryContactEmail,
            PrimaryContactPhone = request.PrimaryContactPhone,
            PrimaryContactTitle = request.PrimaryContactTitle,
            BillingAddressLine1 = request.BillingAddressLine1,
            BillingAddressLine2 = request.BillingAddressLine2,
            BillingCity = request.BillingCity,
            BillingState = request.BillingState,
            BillingPostalCode = request.BillingPostalCode,
            BillingCountry = request.BillingCountry,
            ShippingAddressLine1 = request.ShippingAddressLine1,
            ShippingAddressLine2 = request.ShippingAddressLine2,
            ShippingCity = request.ShippingCity,
            ShippingState = request.ShippingState,
            ShippingPostalCode = request.ShippingPostalCode,
            ShippingCountry = request.ShippingCountry,
            LinkedInProfile = request.LinkedInProfile,
            TwitterHandle = request.TwitterHandle,
            AccountManagerEmployeeId = request.AccountManagerEmployeeId,
            CustomerSegment = request.CustomerSegment,
            CustomerStatus = request.CustomerStatus ?? "Active",
            Tags = request.Tags,
            CreatedDate = DateTime.UtcNow
        };
        _customers.Add(customer);
        return Task.FromResult(MapToDto(customer));
    }

    public Task<CustomerDto> UpdateCustomerAsync(string id, UpdateCustomerRequest request)
    {
        var customer = _customers.FirstOrDefault(c => c.Id == id);
        if (customer == null) throw new KeyNotFoundException($"Customer {id} not found");

        customer.Name = request.Name;
        customer.CompanyName = request.CompanyName;
        customer.Email = request.Email;
        customer.Phone = request.Phone;
        customer.Website = request.Website;
        customer.Industry = request.Industry;
        customer.EmployeeCount = request.EmployeeCount;
        customer.AnnualRevenue = request.AnnualRevenue;
        customer.Description = request.Description;
        customer.PrimaryContactName = request.PrimaryContactName;
        customer.PrimaryContactEmail = request.PrimaryContactEmail;
        customer.PrimaryContactPhone = request.PrimaryContactPhone;
        customer.PrimaryContactTitle = request.PrimaryContactTitle;
        customer.BillingAddressLine1 = request.BillingAddressLine1;
        customer.BillingAddressLine2 = request.BillingAddressLine2;
        customer.BillingCity = request.BillingCity;
        customer.BillingState = request.BillingState;
        customer.BillingPostalCode = request.BillingPostalCode;
        customer.BillingCountry = request.BillingCountry;
        customer.ShippingAddressLine1 = request.ShippingAddressLine1;
        customer.ShippingAddressLine2 = request.ShippingAddressLine2;
        customer.ShippingCity = request.ShippingCity;
        customer.ShippingState = request.ShippingState;
        customer.ShippingPostalCode = request.ShippingPostalCode;
        customer.ShippingCountry = request.ShippingCountry;
        customer.LinkedInProfile = request.LinkedInProfile;
        customer.TwitterHandle = request.TwitterHandle;
        customer.AccountManagerEmployeeId = request.AccountManagerEmployeeId;
        customer.CustomerSegment = request.CustomerSegment;
        customer.CustomerStatus = request.CustomerStatus;
        customer.LifetimeValue = request.LifetimeValue;
        customer.Tags = request.Tags;

        return Task.FromResult(MapToDto(customer));
    }

    public Task DeleteCustomerAsync(string id)
    {
        var customer = _customers.FirstOrDefault(c => c.Id == id);
        if (customer != null) _customers.Remove(customer);
        return Task.CompletedTask;
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            CompanyName = customer.CompanyName,
            Email = customer.Email,
            Phone = customer.Phone,
            Website = customer.Website,
            Industry = customer.Industry,
            EmployeeCount = customer.EmployeeCount,
            AnnualRevenue = customer.AnnualRevenue,
            Description = customer.Description,
            PrimaryContactName = customer.PrimaryContactName,
            PrimaryContactEmail = customer.PrimaryContactEmail,
            PrimaryContactPhone = customer.PrimaryContactPhone,
            PrimaryContactTitle = customer.PrimaryContactTitle,
            BillingAddressLine1 = customer.BillingAddressLine1,
            BillingAddressLine2 = customer.BillingAddressLine2,
            BillingCity = customer.BillingCity,
            BillingState = customer.BillingState,
            BillingPostalCode = customer.BillingPostalCode,
            BillingCountry = customer.BillingCountry,
            ShippingAddressLine1 = customer.ShippingAddressLine1,
            ShippingAddressLine2 = customer.ShippingAddressLine2,
            ShippingCity = customer.ShippingCity,
            ShippingState = customer.ShippingState,
            ShippingPostalCode = customer.ShippingPostalCode,
            ShippingCountry = customer.ShippingCountry,
            LinkedInProfile = customer.LinkedInProfile,
            TwitterHandle = customer.TwitterHandle,
            AccountManagerEmployeeId = customer.AccountManagerEmployeeId,
            CustomerSegment = customer.CustomerSegment,
            CustomerStatus = customer.CustomerStatus ?? "Active",
            CreatedDate = customer.CreatedDate,
            LastContactDate = customer.LastContactDate,
            LifetimeValue = customer.LifetimeValue,
            Tags = customer.Tags,
            OpportunityCount = customer.Opportunities?.Count ?? 0,
            ActivityCount = customer.Activities?.Count ?? 0
        };
    }

    private static List<Customer> GenerateSampleCustomers()
    {
        var now = DateTime.UtcNow;
        return new List<Customer>
        {
            new() { Id = "C1", Name = "TechCorp International", CompanyName = "TechCorp", Email = "contact@techcorp.com", Phone = "555-1001", Industry = "Technology", EmployeeCount = 500, AnnualRevenue = 50000000, CustomerSegment = "Enterprise", CustomerStatus = "Active", LifetimeValue = 450000, CreatedDate = now.AddDays(-120) },
            new() { Id = "C2", Name = "Innovate Inc", CompanyName = "Innovate Inc", Email = "info@innovate.com", Phone = "555-1002", Industry = "SaaS", EmployeeCount = 150, AnnualRevenue = 15000000, CustomerSegment = "Mid-Market", CustomerStatus = "Active", LifetimeValue = 280000, CreatedDate = now.AddDays(-95) },
            new() { Id = "C3", Name = "StartupCo", CompanyName = "StartupCo", Email = "hello@startupco.io", Phone = "555-1003", Industry = "Technology", EmployeeCount = 25, AnnualRevenue = 2000000, CustomerSegment = "SMB", CustomerStatus = "Active", LifetimeValue = 45000, CreatedDate = now.AddDays(-50) },
            new() { Id = "C4", Name = "Enterprise Solutions Ltd", CompanyName = "Enterprise Solutions", Email = "contact@enterprise.com", Phone = "555-1004", Industry = "Consulting", EmployeeCount = 1200, AnnualRevenue = 120000000, CustomerSegment = "Enterprise", CustomerStatus = "Active", LifetimeValue = 950000, CreatedDate = now.AddDays(-180) },
            new() { Id = "C5", Name = "Wilson Consulting", CompanyName = "Wilson Consulting", Email = "info@wilsonconsulting.com", Phone = "555-1005", Industry = "Professional Services", EmployeeCount = 45, AnnualRevenue = 5000000, CustomerSegment = "SMB", CustomerStatus = "Active", LifetimeValue = 120000, CreatedDate = now.AddDays(-70) },
            new() { Id = "C6", Name = "GrowthCo Marketing", CompanyName = "GrowthCo", Email = "sales@growthco.com", Phone = "555-1006", Industry = "Marketing", EmployeeCount = 85, AnnualRevenue = 8500000, CustomerSegment = "Mid-Market", CustomerStatus = "Active", LifetimeValue = 195000, CreatedDate = now.AddDays(-35) },
            new() { Id = "C7", Name = "SmallBiz Services", CompanyName = "SmallBiz", Email = "hello@smallbiz.com", Phone = "555-1007", Industry = "Retail", EmployeeCount = 12, AnnualRevenue = 850000, CustomerSegment = "SMB", CustomerStatus = "Active", LifetimeValue = 25000, CreatedDate = now.AddDays(-15) }
        };
    }
}
