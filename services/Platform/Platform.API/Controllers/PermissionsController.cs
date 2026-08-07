using Microsoft.AspNetCore.Mvc;
using Platform.Application.DTOs;
using Platform.Application.Interfaces;

namespace Platform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;
    private readonly ILogger<PermissionsController> _logger;

    public PermissionsController(IPermissionService permissionService, ILogger<PermissionsController> logger)
    {
        _permissionService = permissionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetAll()
    {
        try
        {
            var permissions = await _permissionService.GetAllPermissionsAsync();
            return Ok(permissions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PermissionDto>> GetById(Guid id)
    {
        try
        {
            var permission = await _permissionService.GetPermissionByIdAsync(id);
            if (permission == null)
            {
                return NotFound($"Permission with ID {id} not found");
            }
            return Ok(permission);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permission {PermissionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("module/{module}")]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetByModule(string module)
    {
        try
        {
            var permissions = await _permissionService.GetPermissionsByModuleAsync(module);
            return Ok(permissions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions for module {Module}", module);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<PermissionDto>> Create([FromBody] CreatePermissionDto createPermissionDto)
    {
        try
        {
            var permission = await _permissionService.CreatePermissionAsync(createPermissionDto);
            return CreatedAtAction(nameof(GetById), new { id = permission.Id }, permission);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating permission");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await _permissionService.DeletePermissionAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting permission {PermissionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
