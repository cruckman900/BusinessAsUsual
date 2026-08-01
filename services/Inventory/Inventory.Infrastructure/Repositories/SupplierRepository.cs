using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly InventoryDbContext _context;

    public SupplierRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Supplier>> GetAllAsync()
        => await _context.Suppliers.Where(s => s.IsActive).ToListAsync();

    public async Task<Supplier?> GetByIdAsync(Guid id)
        => await _context.Suppliers
            .Include(s => s.PurchaseOrders)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<Supplier?> GetByCodeAsync(string code)
        => await _context.Suppliers.FirstOrDefaultAsync(s => s.Code == code);

    public async Task<IEnumerable<Supplier>> SearchAsync(string searchTerm)
        => await _context.Suppliers
            .Where(s => s.IsActive && (
                s.Name.Contains(searchTerm) ||
                (s.Code != null && s.Code.Contains(searchTerm)) ||
                (s.ContactName != null && s.ContactName.Contains(searchTerm))
            ))
            .ToListAsync();

    public async Task<Supplier> AddAsync(Supplier entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        _context.Suppliers.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Supplier> UpdateAsync(Supplier entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Suppliers.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.Suppliers.FindAsync(id);
        if (entity != null)
        {
            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
