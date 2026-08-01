using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly InventoryDbContext _context;

    public ProductRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
        => await _context.Products.Where(p => p.IsActive).ToListAsync();

    public async Task<Product?> GetByIdAsync(Guid id)
        => await _context.Products
            .Include(p => p.StockItems)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Product?> GetBySkuAsync(string sku)
        => await _context.Products.FirstOrDefaultAsync(p => p.SKU == sku);

    public async Task<Product?> GetByBarcodeAsync(string barcode)
        => await _context.Products.FirstOrDefaultAsync(p => p.Barcode == barcode);

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
        => await _context.Products
            .Where(p => p.IsActive && (
                p.Name.Contains(searchTerm) ||
                p.SKU.Contains(searchTerm) ||
                (p.Barcode != null && p.Barcode.Contains(searchTerm))
            ))
            .ToListAsync();

    public async Task<IEnumerable<Product>> GetLowStockAsync()
        => await _context.Products
            .Include(p => p.StockItems)
            .Where(p => p.IsActive && p.IsTrackedInventory &&
                p.StockItems.Sum(s => s.QuantityOnHand) <= p.ReorderPoint)
            .ToListAsync();

    public async Task<Product> AddAsync(Product entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        _context.Products.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Product> UpdateAsync(Product entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Products.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.Products.FindAsync(id);
        if (entity != null)
        {
            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
