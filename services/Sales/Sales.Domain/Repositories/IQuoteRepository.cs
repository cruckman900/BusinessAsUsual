using Sales.Domain.Entities;

namespace Sales.Domain.Repositories;

public interface IQuoteRepository
{
    Task<IEnumerable<Quote>> GetAllAsync();
    Task<Quote?> GetByIdAsync(string id);
    Task<Quote> AddAsync(Quote quote);
    Task<Quote> UpdateAsync(Quote quote);
    Task<bool> DeleteAsync(string id);
}
