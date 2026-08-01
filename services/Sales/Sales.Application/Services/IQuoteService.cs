using Sales.Application.DTOs;

namespace Sales.Application.Services;

public interface IQuoteService
{
    Task<IEnumerable<QuoteDto>> GetAllQuotesAsync();
    Task<QuoteDto?> GetQuoteByIdAsync(string id);
    Task<QuoteDto> CreateQuoteAsync(CreateQuoteDto dto);
    Task<QuoteDto> UpdateQuoteAsync(UpdateQuoteDto dto);
    Task<bool> DeleteQuoteAsync(string id);
    Task<QuoteDto> SendQuoteAsync(string id);
    Task<QuoteDto> AcceptQuoteAsync(string id);
    Task<QuoteDto> RejectQuoteAsync(string id);
    Task<OrderDto> ConvertQuoteToOrderAsync(string quoteId);
}
