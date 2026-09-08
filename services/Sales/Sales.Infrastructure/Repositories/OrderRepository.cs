using Microsoft.EntityFrameworkCore;
using Sales.Domain.Entities;
using Sales.Domain.Repositories;
using Sales.Infrastructure.Persistence;
using BusinessAsUsual.Application.Services;

namespace Sales.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly SalesDbContext _context;
    private readonly ITenantContext _tenantContext;

    public OrderRepository(SalesDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Where(o => o.CompanyId == _tenantContext.CompanyId)
            .Include(o => o.LineItems)
            .Include(o => o.Payments)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(string id)
    {
        return await _context.Orders
            .Include(o => o.LineItems)
            .Include(o => o.Payments)
            .FirstOrDefaultAsync(o => o.Id == id && o.CompanyId == _tenantContext.CompanyId);
    }

    public async Task<Order> AddAsync(Order order)
    {
        order.CompanyId = _tenantContext.CompanyId;
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order> UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> CountAsync()
    {
        return await _context.Orders.CountAsync(o => o.CompanyId == _tenantContext.CompanyId);
    }
}
