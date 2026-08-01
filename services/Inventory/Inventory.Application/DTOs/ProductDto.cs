namespace Inventory.Application.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public decimal Cost { get; set; }
    public decimal Price { get; set; }
    public int ReorderPoint { get; set; }
    public int ReorderQuantity { get; set; }
    public string? Category { get; set; }
    public string UnitOfMeasure { get; set; } = "EA";
    public bool IsActive { get; set; }
    public bool IsTrackedInventory { get; set; }
    public string? ImageUrl { get; set; }
    public int TotalStock { get; set; }
}

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public decimal Cost { get; set; }
    public decimal Price { get; set; }
    public int ReorderPoint { get; set; }
    public int ReorderQuantity { get; set; }
    public string? Category { get; set; }
    public string UnitOfMeasure { get; set; } = "EA";
    public bool IsTrackedInventory { get; set; } = true;
    public string? ImageUrl { get; set; }
}

public class UpdateProductDto : CreateProductDto
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
}
