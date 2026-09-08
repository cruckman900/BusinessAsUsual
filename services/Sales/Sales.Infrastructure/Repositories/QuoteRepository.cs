using Microsoft.EntityFrameworkCore;
using Sales.Domain.Entities;
using Sales.Domain.Repositories;
using Sales.Infrastructure.Persistence;
using BusinessAsUsual.Application.Services;

namespace Sales.Infrastructure.Repositories;

public class QuoteRepository : IQuoteRepository
{
    private readonly SalesDbContext _context;
    private readonly ITenantContext _tenantContext;

    public QuoteRepository(SalesDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<IEnumerable<Quote>> GetAllAsync()
    {
        return await _context.Quotes
            .Where(q => q.CompanyId == _tenantContext.CompanyId)
            .Include(q => q.LineItems)
            .ToListAsync();
    }

    public async Task<Quote?> GetByIdAsync(string id)
    {
        return await _context.Quotes
            .Include(q => q.LineItems)
            .FirstOrDefaultAsync(q => q.Id == id && q.CompanyId == _tenantContext.CompanyId);
    }

    public async Task<Quote> AddAsync(Quote quote)
    {
        quote.CompanyId = _tenantContext.CompanyId;
        _context.Quotes.Add(quote);
        await _context.SaveChangesAsync();
        return quote;
    }

    public async Task<Quote> UpdateAsync(Quote quote)
    {
        _context.Quotes.Update(quote);
        await _context.SaveChangesAsync();
        return quote;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var quote = await _context.Quotes.FindAsync(id);
        if (quote == null) return false;

        _context.Quotes.Remove(quote);
        await _context.SaveChangesAsync();
        return true;
    }
}
