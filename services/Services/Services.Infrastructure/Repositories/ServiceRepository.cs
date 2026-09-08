using Microsoft.EntityFrameworkCore;
using Services.Domain.Entities;
using Services.Domain.Interfaces;
using Services.Infrastructure.Data;
using BusinessAsUsual.Application.Services;

namespace Services.Infrastructure.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly ServicesDbContext _db;
    private readonly ITenantContext _tenantContext;

    public ServiceRepository(ServicesDbContext db, ITenantContext tenantContext)
    {
        _db = db;
        _tenantContext = tenantContext;
    }

    public async Task<List<Service>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Services.AsNoTracking()
            .Where(s => s.CompanyId == _tenantContext.CompanyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Service?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var service = await _db.Services.FindAsync(new object[] { id }, cancellationToken);
        return service != null && service.CompanyId == _tenantContext.CompanyId ? service : null;
    }

    public async Task<Service> CreateAsync(Service service, CancellationToken cancellationToken = default)
    {
        service.CompanyId = _tenantContext.CompanyId;
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
