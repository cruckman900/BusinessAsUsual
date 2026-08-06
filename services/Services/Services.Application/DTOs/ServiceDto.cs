namespace Services.Application.DTOs;
using System.ComponentModel.DataAnnotations;

public class ServiceDto
{
    public Guid Id { get; set; }
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Range(0, 1000000)]
    public decimal BasePrice { get; set; }

    public bool IsActive { get; set; }

    public ServiceDto() { }

    public ServiceDto(Guid id, string name, string? description, decimal basePrice, bool isActive)
    {
        Id = id;
        Name = name;
        Description = description;
        BasePrice = basePrice;
        IsActive = isActive;
    }
}
