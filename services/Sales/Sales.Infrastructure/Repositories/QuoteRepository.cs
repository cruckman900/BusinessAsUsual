using Microsoft.EntityFrameworkCore;
using Sales.Domain.Entities;
using Sales.Domain.Repositories;
using Sales.Infrastructure.Persistence;

namespace Sales.Infrastructure.Repositories;

public class QuoteRepository : IQuoteRepository
{
    private readonly SalesDbContext _context;

    public QuoteRepository(SalesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Quote>> GetAllAsync()
    {
        return await _context.Quotes
            .Include(q => q.LineItems)
            .ToListAsync();
    }

    public async Task<Quote?> GetByIdAsync(string id)
    {
        return await _context.Quotes
            .Include(q => q.LineItems)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<Quote> AddAsync(Quote quote)
    {
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
