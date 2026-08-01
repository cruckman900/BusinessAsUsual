using Inventory.Application.DTOs;
using Inventory.Domain.Interfaces;

namespace Inventory.Application.Services;

public class SupplierService
{
    private readonly ISupplierRepository _supplierRepository;

    public SupplierService(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
    {
        var suppliers = await _supplierRepository.GetAllAsync();
        return suppliers.Select(s => new SupplierDto
        {
            Id = s.Id,
            Name = s.Name,
            Code = s.Code,
            ContactName = s.ContactName,
            Email = s.Email,
            Phone = s.Phone,
            Address = s.Address,
            City = s.City,
            State = s.State,
            ZipCode = s.ZipCode,
            Country = s.Country,
            IsActive = s.IsActive
        });
    }
}
