using Finance.Application.DTOs;
using Finance.Application.Services;
using Finance.Domain.Enums;
using FluentAssertions;

namespace Finance.Tests.Unit;

public class BillServiceTests
{
    private static MockBillService NewService()
    {
        return new MockBillService();
    }

    [Fact]
    public async Task GetAllBillsAsync_ShouldReturnSeededData()
    {
        // Arrange
        var service = NewService();

        // Act
        var bills = (await service.GetAllBillsAsync()).ToList();

        // Assert
        bills.Should().NotBeEmpty();
        bills.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetBillByIdAsync_WithValidId_ShouldReturnBill()
    {
        // Arrange
        var service = NewService();
        var bills = (await service.GetAllBillsAsync()).ToList();
        var firstBill = bills.First();

        // Act
        var result = await service.GetBillByIdAsync(firstBill.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(firstBill.Id);
    }

    [Fact]
    public async Task CreateBillAsync_ShouldCreateWithLineItems()
    {
        // Arrange
        var service = NewService();
        var request = new CreateBillRequest
        {
            VendorName = "Test Vendor",
            VendorEmail = "test@vendor.com",
            DueDate = DateTime.UtcNow.AddDays(30),
            Terms = "Net 30",
            Currency = Currency.USD,
            LineItems = new List<CreateBillLineItemRequest>
            {
                new CreateBillLineItemRequest
                {
                    Description = "Test Item",
                    Quantity = 2,
                    UnitPrice = 50m,
                    TaxPercent = 8m,
                    ExpenseCategory = "Test Category"
                }
            }
        };

        // Act
        var result = await service.CreateBillAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.VendorName.Should().Be("Test Vendor");
        result.LineItems.Should().HaveCount(1);
        result.Status.Should().Be(BillStatus.Draft.ToString());
    }

    [Fact]
    public async Task UpdateBillAsync_ShouldChangeStatus()
    {
        // Arrange
        var service = NewService();
        var createRequest = new CreateBillRequest
        {
            VendorName = "Draft Vendor",
            VendorEmail = "draft@vendor.com",
            DueDate = DateTime.UtcNow.AddDays(30),
            Terms = "Net 30",
            Currency = Currency.USD,
            LineItems = new List<CreateBillLineItemRequest>
            {
                new CreateBillLineItemRequest
                {
                    Description = "Draft Item",
                    Quantity = 1,
                    UnitPrice = 100m,
                    TaxPercent = 0m,
                    ExpenseCategory = "Test"
                }
            }
        };
        var draftBill = await service.CreateBillAsync(createRequest);

        var updateRequest = new UpdateBillRequest
        {
            VendorName = draftBill.VendorName,
            VendorEmail = draftBill.VendorEmail,
            Status = BillStatus.Received,
            Currency = Currency.USD,
            DueDate = draftBill.DueDate,
            Terms = draftBill.Terms,
            LineItems = new List<CreateBillLineItemRequest>
            {
                new CreateBillLineItemRequest
                {
                    Description = "Draft Item",
                    Quantity = 1,
                    UnitPrice = 100m,
                    TaxPercent = 0m,
                    ExpenseCategory = "Test"
                }
            }
        };

        // Act
        var result = await service.UpdateBillAsync(draftBill.Id, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(BillStatus.Received.ToString());
    }

    [Fact]
    public async Task UpdateBillAsync_ToPaidStatus_ShouldSetPaidDate()
    {
        // Arrange
        var service = NewService();
        var bills = (await service.GetAllBillsAsync()).ToList();
        var unpaidBill = bills.FirstOrDefault(b => b.Status == BillStatus.Received.ToString());

        if (unpaidBill == null)
        {
            // Create and receive a bill first
            var createRequest = new CreateBillRequest
            {
                VendorName = "Payable Vendor",
                VendorEmail = "payable@vendor.com",
                DueDate = DateTime.UtcNow.AddDays(30),
                Terms = "Net 30",
                Currency = Currency.USD,
                LineItems = new List<CreateBillLineItemRequest>
                {
                    new CreateBillLineItemRequest
                    {
                        Description = "Payable Item",
                        Quantity = 1,
                        UnitPrice = 200m,
                        TaxPercent = 0m,
                        ExpenseCategory = "Test"
                    }
                }
            };
            var draftBill = await service.CreateBillAsync(createRequest);
            var updateToReceived = new UpdateBillRequest
            {
                VendorName = draftBill.VendorName,
                VendorEmail = draftBill.VendorEmail,
                Status = BillStatus.Received,
                Currency = Currency.USD,
                DueDate = draftBill.DueDate,
                Terms = draftBill.Terms,
                LineItems = new List<CreateBillLineItemRequest>
                {
                    new CreateBillLineItemRequest
                    {
                        Description = "Payable Item",
                        Quantity = 1,
                        UnitPrice = 200m,
                        TaxPercent = 0m,
                        ExpenseCategory = "Test"
                    }
                }
            };
            unpaidBill = await service.UpdateBillAsync(draftBill.Id, updateToReceived);
        }

        var updateRequest = new UpdateBillRequest
        {
            VendorName = unpaidBill.VendorName,
            VendorEmail = unpaidBill.VendorEmail,
            Status = BillStatus.Paid,
            Currency = Enum.Parse<Currency>(unpaidBill.Currency),
            DueDate = unpaidBill.DueDate,
            Terms = unpaidBill.Terms,
            LineItems = new List<CreateBillLineItemRequest>
            {
                new CreateBillLineItemRequest
                {
                    Description = "Payable Item",
                    Quantity = 1,
                    UnitPrice = 200m,
                    TaxPercent = 0m,
                    ExpenseCategory = "Test"
                }
            }
        };

        // Act
        var result = await service.UpdateBillAsync(unpaidBill.Id, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(BillStatus.Paid.ToString());
        result.PaidDate.Should().NotBeNull();
    }
}
