using Inventory.Application.DTOs;
using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;

namespace Inventory.Application.Services;

public class WarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IStockItemRepository _stockItemRepository;

    public WarehouseService(IWarehouseRepository warehouseRepository, IStockItemRepository stockItemRepository)
    {
        _warehouseRepository = warehouseRepository;
        _stockItemRepository = stockItemRepository;
    }

    public async Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync()
    {
        var warehouses = await _warehouseRepository.GetAllAsync();
        var warehouseDtos = new List<WarehouseDto>();

        foreach (var warehouse in warehouses)
        {
            var stockItems = await _stockItemRepository.GetByWarehouseIdAsync(warehouse.Id);

            warehouseDtos.Add(new WarehouseDto
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Code = warehouse.Code,
                Address = warehouse.Address,
                City = warehouse.City,
                State = warehouse.State,
                ZipCode = warehouse.ZipCode,
                Country = warehouse.Country,
                ManagerName = warehouse.ManagerName,
                Phone = warehouse.Phone,
                Email = warehouse.Email,
                IsActive = warehouse.IsActive,
                TotalStockItems = stockItems.Count(),
                BinLocationCount = warehouse.BinLocations.Count
            });
        }

        return warehouseDtos;
    }

    public async Task<WarehouseDto?> GetWarehouseByIdAsync(Guid id)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(id);
        if (warehouse == null) return null;

        var stockItems = await _stockItemRepository.GetByWarehouseIdAsync(id);

        return new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Code = warehouse.Code,
            Address = warehouse.Address,
            City = warehouse.City,
            State = warehouse.State,
            ZipCode = warehouse.ZipCode,
            Country = warehouse.Country,
            ManagerName = warehouse.ManagerName,
            Phone = warehouse.Phone,
            Email = warehouse.Email,
            IsActive = warehouse.IsActive,
            TotalStockItems = stockItems.Count(),
            BinLocationCount = warehouse.BinLocations.Count
        };
    }

    public async Task<WarehouseDto> CreateWarehouseAsync(CreateWarehouseDto dto)
    {
        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Code = dto.Code,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            ZipCode = dto.ZipCode,
            Country = dto.Country,
            ManagerName = dto.ManagerName,
            Phone = dto.Phone,
            Email = dto.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _warehouseRepository.AddAsync(warehouse);

        return new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Code = warehouse.Code,
            Address = warehouse.Address,
            City = warehouse.City,
            State = warehouse.State,
            ZipCode = warehouse.ZipCode,
            Country = warehouse.Country,
            ManagerName = warehouse.ManagerName,
            Phone = warehouse.Phone,
            Email = warehouse.Email,
            IsActive = warehouse.IsActive,
            TotalStockItems = 0,
            BinLocationCount = 0
        };
    }

    public async Task<WarehouseDto> UpdateWarehouseAsync(UpdateWarehouseDto dto)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(dto.Id);
        if (warehouse == null)
            throw new Exception($"Warehouse with ID {dto.Id} not found");

        warehouse.Name = dto.Name;
        warehouse.Code = dto.Code;
        warehouse.Address = dto.Address;
        warehouse.City = dto.City;
        warehouse.State = dto.State;
        warehouse.ZipCode = dto.ZipCode;
        warehouse.Country = dto.Country;
        warehouse.ManagerName = dto.ManagerName;
        warehouse.Phone = dto.Phone;
        warehouse.Email = dto.Email;
        warehouse.IsActive = dto.IsActive;
        warehouse.UpdatedAt = DateTime.UtcNow;

        await _warehouseRepository.UpdateAsync(warehouse);

        var stockItems = await _stockItemRepository.GetByWarehouseIdAsync(warehouse.Id);

        return new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Code = warehouse.Code,
            Address = warehouse.Address,
            City = warehouse.City,
            State = warehouse.State,
            ZipCode = warehouse.ZipCode,
            Country = warehouse.Country,
            ManagerName = warehouse.ManagerName,
            Phone = warehouse.Phone,
            Email = warehouse.Email,
            IsActive = warehouse.IsActive,
            TotalStockItems = stockItems.Count(),
            BinLocationCount = warehouse.BinLocations.Count
        };
    }

    public async Task DeleteWarehouseAsync(Guid id)
    {
        await _warehouseRepository.DeleteAsync(id);
    }
}
