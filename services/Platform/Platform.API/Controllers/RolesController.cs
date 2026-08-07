using Microsoft.AspNetCore.Mvc;
using Platform.Application.DTOs;
using Platform.Application.Interfaces;

namespace Platform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly ILogger<RolesController> _logger;

    public RolesController(IRoleService roleService, ILogger<RolesController> logger)
    {
        _roleService = roleService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll()
    {
        try
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoleDto>> GetById(Guid id)
    {
        try
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound($"Role with ID {id} not found");
            }
            return Ok(role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role {RoleId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleDto createRoleDto)
    {
        try
        {
            var role = await _roleService.CreateRoleAsync(createRoleDto);
            return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RoleDto>> Update(Guid id, [FromBody] UpdateRoleDto updateRoleDto)
    {
        try
        {
            var role = await _roleService.UpdateRoleAsync(id, updateRoleDto);
            return Ok(role);
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
            _logger.LogError(ex, "Error updating role {RoleId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await _roleService.DeleteRoleAsync(id);
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
            _logger.LogError(ex, "Error deleting role {RoleId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetByUser(Guid userId)
    {
        try
        {
            var roles = await _roleService.GetRolesByUserAsync(userId);
            return Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles by user {UserId}", userId);
            return StatusCode(500, "Internal server error");
        }
    }
}
