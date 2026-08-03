using FluentAssertions;
using Moq;
using Sales.Application.DTOs;
using Sales.Application.Services;
using Sales.Domain.Entities;
using Sales.Domain.Enums;
using Sales.Domain.Repositories;
using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Core.Events.Integration;

namespace Sales.Tests.Services;

public class QuoteServiceTests
{
    private readonly Mock<IQuoteRepository> _quoteRepositoryMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly QuoteService _service;

    public QuoteServiceTests()
    {
        _quoteRepositoryMock = new Mock<IQuoteRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _eventBusMock = new Mock<IEventBus>();
        _service = new QuoteService(_quoteRepositoryMock.Object, _orderRepositoryMock.Object, _eventBusMock.Object);
    }

    [Fact]
    public async Task GetAllQuotesAsync_ShouldReturnAllQuotes()
    {
        // Arrange
        var quotes = new List<Quote>
        {
            new() { Id = "1", QuoteNumber = "QTE-001", CustomerName = "Test Customer", LineItems = new() },
            new() { Id = "2", QuoteNumber = "QTE-002", CustomerName = "Another Customer", LineItems = new() }
        };
        _quoteRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(quotes);

        // Act
        var result = await _service.GetAllQuotesAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllBeOfType<QuoteDto>();
    }

    [Fact]
    public async Task GetQuoteByIdAsync_WithValidId_ShouldReturnQuote()
    {
        // Arrange
        var quote = new Quote 
        { 
            Id = "1", 
            QuoteNumber = "QTE-001", 
            CustomerName = "Test Customer",
            LineItems = new()
        };
        _quoteRepositoryMock.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(quote);

        // Act
        var result = await _service.GetQuoteByIdAsync("1");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("1");
        result.QuoteNumber.Should().Be("QTE-001");
    }

    [Fact]
    public async Task GetQuoteByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _quoteRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((Quote?)null);

        // Act
        var result = await _service.GetQuoteByIdAsync("INVALID_ID");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateQuoteAsync_ShouldCreateQuote()
    {
        // Arrange
        var request = new CreateQuoteDto
        {
            CustomerId = "C1",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            CustomerPhone = "555-1234",
            ExpiryDate = DateTime.UtcNow.AddDays(30),
            Notes = "Test quote",
            LineItems = new List<CreateQuoteLineItemDto>
            {
                new()
                {
                    ProductId = "P1",
                    ProductName = "Test Product",
                    Quantity = 2,
                    UnitPrice = 50.00m,
                    DiscountPercentage = 10,
                    TaxPercentage = 8.5m
                }
            }
        };

        _quoteRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Quote>());
        _quoteRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Quote>()))
            .ReturnsAsync((Quote q) => q);

        // Act
        var result = await _service.CreateQuoteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrEmpty();
        result.QuoteNumber.Should().StartWith("Q-");
        result.CustomerId.Should().Be(request.CustomerId);
        result.CustomerName.Should().Be(request.CustomerName);
        result.Status.Should().Be(QuoteStatus.Draft);
        result.LineItems.Should().HaveCount(1);
        result.Total.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task UpdateQuoteAsync_WithValidId_ShouldUpdateQuote()
    {
        // Arrange
        var existingQuote = new Quote
        {
            Id = "1",
            QuoteNumber = "QTE-001",
            CustomerId = "C1",
            CustomerName = "Old Name",
            LineItems = new()
        };

        var updateRequest = new UpdateQuoteDto
        {
            Id = "1",
            CustomerId = "C1",
            CustomerName = "Updated Customer Name",
            CustomerEmail = "updated@test.com",
            ExpiryDate = DateTime.UtcNow.AddDays(60),
            Notes = "Updated notes",
            LineItems = new List<CreateQuoteLineItemDto>()
        };

        _quoteRepositoryMock.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(existingQuote);
        _quoteRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Quote>()))
            .ReturnsAsync((Quote q) => q);

        // Act
        var result = await _service.UpdateQuoteAsync(updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.CustomerName.Should().Be("Updated Customer Name");
        result.Notes.Should().Be("Updated notes");
    }

    [Fact]
    public async Task UpdateQuoteAsync_WithInvalidId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var updateRequest = new UpdateQuoteDto
        {
            Id = "INVALID_ID",
            CustomerId = "C1",
            CustomerName = "Test",
            CustomerEmail = "test@test.com",
            ExpiryDate = DateTime.UtcNow.AddDays(30),
            LineItems = new List<CreateQuoteLineItemDto>()
        };

        _quoteRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((Quote?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.UpdateQuoteAsync(updateRequest)
        );
    }

    [Fact]
    public async Task SendQuoteAsync_ShouldChangeStatus()
    {
        // Arrange
        var quote = new Quote
        {
            Id = "1",
            QuoteNumber = "QTE-001",
            CustomerId = "C1",
            CustomerName = "Test Customer",
            Status = QuoteStatus.Draft,
            LineItems = new()
        };

        _quoteRepositoryMock.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(quote);
        _quoteRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Quote>()))
            .ReturnsAsync((Quote q) => q);

        // Act
        var result = await _service.SendQuoteAsync("1");

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(QuoteStatus.Sent);
        result.SentDate.Should().NotBeNull();
    }

    [Fact]
    public async Task AcceptQuoteAsync_ShouldChangeStatus()
    {
        // Arrange
        var quote = new Quote
        {
            Id = "1",
            QuoteNumber = "QTE-001",
            Status = QuoteStatus.Sent,
            LineItems = new()
        };

        _quoteRepositoryMock.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(quote);
        _quoteRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Quote>()))
            .ReturnsAsync((Quote q) => q);

        // Act
        var result = await _service.AcceptQuoteAsync("1");

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(QuoteStatus.Accepted);
        result.AcceptedDate.Should().NotBeNull();
    }

    [Fact]
    public async Task RejectQuoteAsync_ShouldChangeStatus()
    {
        // Arrange
        var quote = new Quote
        {
            Id = "1",
            QuoteNumber = "QTE-001",
            Status = QuoteStatus.Sent,
            LineItems = new()
        };

        _quoteRepositoryMock.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(quote);
        _quoteRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Quote>()))
            .ReturnsAsync((Quote q) => q);

        // Act
        var result = await _service.RejectQuoteAsync("1");

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(QuoteStatus.Rejected);
    }

    [Fact]
    public async Task ConvertQuoteToOrderAsync_ShouldCreateOrder()
    {
        // Arrange
        var quote = new Quote
        {
            Id = "Q1",
            QuoteNumber = "QTE-001",
            CustomerId = "C1",
            CustomerName = "Test Customer",
            Status = QuoteStatus.Accepted,
            LineItems = new List<QuoteLineItem>
            {
                new()
                {
                    ProductId = "P1",
                    ProductName = "Product 1",
                    Quantity = 2,
                    UnitPrice = 100m
                }
            }
        };

        _quoteRepositoryMock.Setup(x => x.GetByIdAsync("Q1")).ReturnsAsync(quote);
        _orderRepositoryMock.Setup(x => x.CountAsync()).ReturnsAsync(5);
        _orderRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => o);

        // Act
        var result = await _service.ConvertQuoteToOrderAsync("Q1");

        // Assert
        result.Should().NotBeNull();
        result.OrderNumber.Should().StartWith("O-");
        result.Status.Should().Be(OrderStatus.Pending);
        result.CustomerId.Should().Be(quote.CustomerId);
        result.LineItems.Should().HaveCount(quote.LineItems.Count);

        // Verify order was created
        _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task DeleteQuoteAsync_ShouldCallRepository()
    {
        // Arrange
        _quoteRepositoryMock.Setup(x => x.DeleteAsync("1")).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteQuoteAsync("1");

        // Assert
        result.Should().BeTrue();
        _quoteRepositoryMock.Verify(x => x.DeleteAsync("1"), Times.Once);
    }
}

