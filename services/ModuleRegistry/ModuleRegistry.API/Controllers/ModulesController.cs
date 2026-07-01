using Microsoft.AspNetCore.Mvc;
using ModuleRegistry.Application.DTOs;
using ModuleRegistry.Application.Services;

namespace ModuleRegistry.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModulesController : ControllerBase
{
    private readonly IModuleRegistryService _service;

    public ModulesController(IModuleRegistryService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get all registered modules
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ModuleDto>>> GetAllModules()
    {
        var modules = await _service.GetAllModulesAsync();
        return Ok(modules);
    }

    /// <summary>
    /// Get a specific module by ID
    /// </summary>
    [HttpGet("{moduleId}")]
    public async Task<ActionResult<ModuleDto>> GetModule(string moduleId)
    {
        var module = await _service.GetModuleByIdAsync(moduleId);

        if (module == null)
            return NotFound();

        return Ok(module);
    }

    /// <summary>
    /// Get all active modules
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<ModuleDto>>> GetActiveModules()
    {
        var modules = await _service.GetActiveModulesAsync();
        return Ok(modules);
    }

    /// <summary>
    /// Get modules that have UI components
    /// </summary>
    [HttpGet("ui")]
    public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModulesWithUi()
    {
        var modules = await _service.GetModulesWithUiAsync();
        return Ok(modules);
    }

    /// <summary>
    /// Get modules that support mobile with UI specifications
    /// </summary>
    [HttpGet("mobile")]
    public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModulesWithMobile()
    {
        var modules = await _service.GetModulesWithMobileAsync();
        return Ok(modules);
    }

    /// <summary>
    /// Register or update a module
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult> RegisterModule([FromBody] RegisterModuleRequest request)
    {
        await _service.RegisterModuleAsync(request);
        return Ok(new { message = "Module registered successfully" });
    }
}
