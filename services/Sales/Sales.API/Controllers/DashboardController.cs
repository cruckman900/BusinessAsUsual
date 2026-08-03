using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.Infrastructure.Persistence;

namespace Sales.API.Controllers;

[ApiController]
[Route("api/sales/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly SalesDbContext _context;

    public DashboardController(SalesDbContext context)
    {
        _context = context;
    }

    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStats>> GetStats()
    {
        var stats = new DashboardStats
        {
            ActiveQuotes = await _context.Quotes.CountAsync(q => q.Status == Domain.Enums.QuoteStatus.Sent || q.Status == Domain.Enums.QuoteStatus.Viewed),
            OpenOrders = await _context.Orders.CountAsync(o => o.Status == Domain.Enums.OrderStatus.Pending || o.Status == Domain.Enums.OrderStatus.Confirmed || o.Status == Domain.Enums.OrderStatus.Processing),
            MonthlyRevenue = await CalculateMonthlyRevenue(),
            PendingShipments = await _context.Orders.CountAsync(o => o.Status == Domain.Enums.OrderStatus.Confirmed || o.Status == Domain.Enums.OrderStatus.Processing),
            TotalQuotes = await _context.Quotes.CountAsync(),
            TotalOrders = await _context.Orders.CountAsync(),
            AverageOrderValue = await CalculateAverageOrderValue(),
            ConversionRate = await CalculateConversionRate()
        };

        return Ok(stats);
    }

    private async Task<decimal> CalculateMonthlyRevenue()
    {
        var firstDayOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var orders = await _context.Orders
            .Include(o => o.LineItems)
            .Include(o => o.Payments)
            .Where(o => o.OrderDate >= firstDayOfMonth)
            .ToListAsync();

        return orders.Sum(o => o.Payments.Where(p => p.IsCompleted).Sum(p => p.Amount));
    }

    private async Task<decimal> CalculateAverageOrderValue()
    {
        var orders = await _context.Orders
            .Include(o => o.LineItems)
            .ToListAsync();

        if (orders.Count == 0) return 0;

        var totalValue = orders.Sum(o => o.LineItems.Sum(li => 
            li.Quantity * li.UnitPrice * (1 - li.DiscountPercentage / 100) * (1 + li.TaxPercentage / 100)));

        return totalValue / orders.Count;
    }

    private async Task<decimal> CalculateConversionRate()
    {
        var totalQuotes = await _context.Quotes.CountAsync();
        if (totalQuotes == 0) return 0;

        var convertedQuotes = await _context.Quotes.CountAsync(q => q.Status == Domain.Enums.QuoteStatus.Accepted || q.Status == Domain.Enums.QuoteStatus.Converted);

        return (decimal)convertedQuotes / totalQuotes * 100;
    }
}

public class DashboardStats
{
    public int ActiveQuotes { get; set; }
    public int OpenOrders { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public int PendingShipments { get; set; }
    public int TotalQuotes { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal ConversionRate { get; set; }
}
