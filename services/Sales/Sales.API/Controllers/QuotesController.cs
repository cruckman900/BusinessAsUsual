using Sales.Application.DTOs;
using Sales.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Sales.API.Controllers;

[ApiController]
[Route("api/sales/[controller]")]
public class QuotesController : ControllerBase
{
    private readonly IQuoteService _quoteService;

    public QuotesController(IQuoteService quoteService)
    {
        _quoteService = quoteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuoteDto>>> GetAll()
    {
        var quotes = await _quoteService.GetAllQuotesAsync();
        return Ok(quotes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuoteDto>> GetById(string id)
    {
        var quote = await _quoteService.GetQuoteByIdAsync(id);
        if (quote == null)
            return NotFound();
        return Ok(quote);
    }

    [HttpPost]
    public async Task<ActionResult<QuoteDto>> Create([FromBody] CreateQuoteDto dto)
    {
        var quote = await _quoteService.CreateQuoteAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = quote.Id }, quote);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<QuoteDto>> Update(string id, [FromBody] UpdateQuoteDto dto)
    {
        if (id != dto.Id)
            return BadRequest();

        try
        {
            var quote = await _quoteService.UpdateQuoteAsync(dto);
            return Ok(quote);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var result = await _quoteService.DeleteQuoteAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [HttpPost("{id}/send")]
    public async Task<ActionResult<QuoteDto>> Send(string id)
    {
        try
        {
            var quote = await _quoteService.SendQuoteAsync(id);
            return Ok(quote);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("{id}/accept")]
    public async Task<ActionResult<QuoteDto>> Accept(string id)
    {
        try
        {
            var quote = await _quoteService.AcceptQuoteAsync(id);
            return Ok(quote);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("{id}/reject")]
    public async Task<ActionResult<QuoteDto>> Reject(string id)
    {
        try
        {
            var quote = await _quoteService.RejectQuoteAsync(id);
            return Ok(quote);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("{id}/convert")]
    public async Task<ActionResult<OrderDto>> ConvertToOrder(string id)
    {
        try
        {
            var order = await _quoteService.ConvertQuoteToOrderAsync(id);
            return Ok(order);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
