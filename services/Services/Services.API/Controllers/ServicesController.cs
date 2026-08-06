using Microsoft.AspNetCore.Mvc;
using Services.Domain.Entities;
using Services.Application.DTOs;
using Services.Domain.Interfaces;

namespace Services.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceRepository _repo;

    public ServicesController(IServiceRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> Get()
    {
        var list = await _repo.GetAllAsync();
        return Ok(list.Select(s => new ServiceDto(s.Id, s.Name, s.Description, s.BasePrice, s.IsActive)));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceDto>> Get(Guid id)
    {
        var s = await _repo.GetByIdAsync(id);
        if (s == null) return NotFound();
        return Ok(new ServiceDto(s.Id, s.Name, s.Description, s.BasePrice, s.IsActive));
    }

    [HttpPost]
    public async Task<ActionResult<ServiceDto>> Create([FromBody] ServiceDto dto)
    {
        var s = new Service
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            BasePrice = dto.BasePrice,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };
        await _repo.CreateAsync(s);
        var res = new ServiceDto(s.Id, s.Name, s.Description, s.BasePrice, s.IsActive);
        return CreatedAtAction(nameof(Get), new { id = s.Id }, res);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ServiceDto dto)
    {
        var s = await _repo.GetByIdAsync(id);
        if (s == null) return NotFound();
        s.Name = dto.Name;
        s.Description = dto.Description;
        s.BasePrice = dto.BasePrice;
        s.IsActive = dto.IsActive;
        s.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(s);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _repo.DeleteAsync(id);
        return NoContent();
    }
}
