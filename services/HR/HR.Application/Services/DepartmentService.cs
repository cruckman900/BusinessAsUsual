using HR.Application.DTOs;
using HR.Domain.Entities;
using HR.Domain.Repositories;

namespace HR.Application.Services;

/// <summary>
/// Department service implementing business logic
/// </summary>
public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;

    public DepartmentService(IDepartmentRepository repository, IEmployeeRepository employeeRepository)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync()
    {
        var departments = await _repository.GetAllAsync();
        return await Task.WhenAll(departments.Select(d => MapToDtoAsync(d)));
    }

    public async Task<DepartmentDto?> GetDepartmentByIdAsync(string id)
    {
        var department = await _repository.GetByIdAsync(id);
        return department == null ? null : await MapToDtoAsync(department);
    }

    public async Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentRequest request)
    {
#pragma warning disable CS0618 // Type or member is obsolete - setting legacy fields for backward compatibility
        var department = new Department
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Description = request.Description,
            Code = request.Code,
            Location = request.Location,
            CostCenter = request.CostCenter,
            ParentDepartmentId = request.ParentDepartmentId,
            ManagerEmployeeId = request.ManagerEmployeeId,  // Legacy
            EmployeeCount = 0,  // Legacy
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };
#pragma warning restore CS0618

        var created = await _repository.CreateAsync(department);

        // Add managers if specified
        if (request.ManagerIds != null && request.ManagerIds.Any())
        {
            foreach (var managerId in request.ManagerIds)
            {
                var manager = await _employeeRepository.GetByIdAsync(managerId);
                if (manager != null)
                {
                    created.DepartmentManagers.Add(new DepartmentManager
                    {
                        DepartmentId = created.Id,
                        ManagerId = managerId,
                        IsPrimary = managerId == request.ManagerIds.First(),  // First one is primary
                        StartDate = DateTime.UtcNow
                    });
                }
            }
            await _repository.UpdateAsync(created);
        }

        return await MapToDtoAsync(created);
    }

    public async Task<DepartmentDto> UpdateDepartmentAsync(string id, UpdateDepartmentRequest request)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Department with ID {id} not found");
        }

#pragma warning disable CS0618 // Type or member is obsolete - setting legacy ManagerEmployeeId field for backward compatibility
        existing.Name = request.Name;
        existing.Description = request.Description;
        existing.Code = request.Code;
        existing.Location = request.Location;
        existing.CostCenter = request.CostCenter;
        existing.ParentDepartmentId = request.ParentDepartmentId;
        existing.ManagerEmployeeId = request.ManagerEmployeeId;  // Legacy
        existing.IsActive = request.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;
#pragma warning restore CS0618

        // Update managers if specified
        if (request.ManagerIds != null && request.ManagerIds.Any())
        {
            // Remove existing active managers
            var existingManagers = existing.DepartmentManagers.Where(dm => dm.EndDate == null).ToList();
            foreach (var mgr in existingManagers)
            {
                mgr.EndDate = DateTime.UtcNow;
            }

            // Add new managers
            foreach (var managerId in request.ManagerIds)
            {
                var manager = await _employeeRepository.GetByIdAsync(managerId);
                if (manager != null)
                {
                    existing.DepartmentManagers.Add(new DepartmentManager
                    {
                        DepartmentId = existing.Id,
                        ManagerId = managerId,
                        IsPrimary = managerId == request.ManagerIds.First(),
                        StartDate = DateTime.UtcNow
                    });
                }
            }
        }

        var updated = await _repository.UpdateAsync(existing);
        return await MapToDtoAsync(updated);
    }

    public async Task DeleteDepartmentAsync(string id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<DepartmentDto>> SearchDepartmentsAsync(string searchTerm)
    {
        var allDepartments = await _repository.GetAllAsync();
        var filtered = allDepartments.Where(d => 
            d.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
            (d.Description != null && d.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
        return await Task.WhenAll(filtered.Select(d => MapToDtoAsync(d)));
    }

    private async Task<DepartmentDto> MapToDtoAsync(Department department)
    {
        // Get all employees for this department (active assignments only)
        var allEmployees = await _employeeRepository.GetAllAsync();
        var employeeCount = allEmployees.Count(e => 
            e.EmployeeDepartments.Any(ed => 
                ed.DepartmentId == department.Id && 
                ed.LeftDate == null));

        // Get active managers
        var activeManagers = department.DepartmentManagers
            .Where(dm => dm.EndDate == null)
            .Select(dm => new DepartmentManagerDto
            {
                ManagerId = dm.ManagerId,
                ManagerName = dm.Manager != null ? $"{dm.Manager.FirstName} {dm.Manager.LastName}" : "Unknown",
                ManagerRole = dm.ManagerRole,
                IsPrimary = dm.IsPrimary,
                StartDate = dm.StartDate
            })
            .OrderByDescending(m => m.IsPrimary)
            .ThenBy(m => m.ManagerName)
            .ToList();

#pragma warning disable CS0618 // Type or member is obsolete - using legacy Manager property for fallback compatibility
        // Get primary manager for legacy fields
        var primaryManager = department.DepartmentManagers
            .Where(dm => dm.IsPrimary && dm.EndDate == null)
            .Select(dm => dm.Manager)
            .FirstOrDefault() ?? department.Manager;
#pragma warning restore CS0618

        return new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            Code = department.Code,
            Location = department.Location,
            CostCenter = department.CostCenter,
            ParentDepartmentId = department.ParentDepartmentId,
            ParentDepartmentName = department.ParentDepartment?.Name,
            FullPath = department.FullPath,
#pragma warning disable CS0618 // Type or member is obsolete - setting legacy ManagerEmployeeId field for backward compatibility
            ManagerEmployeeId = primaryManager?.Id ?? department.ManagerEmployeeId,  // Legacy
#pragma warning restore CS0618
            ManagerName = primaryManager != null ? $"{primaryManager.FirstName} {primaryManager.LastName}" : null,  // Legacy
            Managers = activeManagers,
            EmployeeCount = employeeCount,  // Dynamically calculated
            SubDepartmentCount = department.SubDepartments?.Count ?? 0,
            IsActive = department.IsActive,
            IsSubDepartment = department.IsSubDepartment,
            CreatedAt = department.CreatedAt
        };
    }
}
