using Microsoft.EntityFrameworkCore;
using Services.Domain.Entities;
using Services.Infrastructure.Data;

namespace Services.Infrastructure.Seeding;

public class DataSeeder
{
    private readonly ServicesDbContext _db;

    public DataSeeder(ServicesDbContext db)
    {
        _db = db;
    }

    public async Task SeedAsync()
    {
        if (await _db.Services.AnyAsync())
            return;

        var now = DateTime.UtcNow;
        var samples = new List<Service>
        {
            new Service { Id = Guid.NewGuid(), Name = "Standard Consultation", Description = "A standard 30-minute consultation.", BasePrice = 49.99M, IsActive = true, CreatedAt = now },
            new Service { Id = Guid.NewGuid(), Name = "Premium Consultation", Description = "A 60-minute deep-dive session.", BasePrice = 99.99M, IsActive = true, CreatedAt = now },
            new Service { Id = Guid.NewGuid(), Name = "On-site Visit", Description = "Travel to customer site for assessment.", BasePrice = 199.99M, IsActive = true, CreatedAt = now }
        };

        _db.Services.AddRange(samples);
        await _db.SaveChangesAsync();
    }
}
