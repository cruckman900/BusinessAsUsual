using Microsoft.EntityFrameworkCore;
using Services.Domain.Entities;
using Services.Domain.Interfaces;
using Services.Infrastructure.Data;

namespace Services.Infrastructure.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly ServicesDbContext _db;

    public ServiceRepository(ServicesDbContext db)
    {
        _db = db;
    }

    public async Task<List<Service>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Services.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Service?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Services.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Service> CreateAsync(Service service, CancellationToken cancellationToken = default)
    {
        _db.Services.Add(service);
        await _db.SaveChangesAsync(cancellationToken);
        return service;
    }

    public async Task UpdateAsync(Service service, CancellationToken cancellationToken = default)
    {
        _db.Services.Update(service);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var s = await GetByIdAsync(id, cancellationToken);
        if (s == null) return;
        _db.Services.Remove(s);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
