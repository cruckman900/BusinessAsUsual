using HR.Infrastructure.Data;
using HR.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR.API.Controllers;

/// <summary>
/// Dev/demo-only endpoint that clears and reseeds the in-memory HR database so the
/// application can present a fresh, fully-populated "tenant" dataset every time a
/// user logs in with the admin/password demo credentials.
/// </summary>
[ApiController]
[Route("api/hr/tenant-reset")]
public class TenantResetController : ControllerBase
{
    private readonly HRDbContext _context;
    private readonly ILogger<TenantResetController> _logger;

    public TenantResetController(HRDbContext context, ILogger<TenantResetController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ResetAsync()
    {
        if (_context.Database.IsRelational())
        {
            _logger.LogWarning("Tenant reset requested but HR database is relational; skipping destructive reset.");
            return BadRequest("Tenant reset is only supported for the in-memory demo database.");
        }

        _logger.LogInformation("Resetting HR tenant database...");

        _context.TrainingCompletions.RemoveRange(_context.TrainingCompletions);
        _context.EmployeeDepartments.RemoveRange(_context.EmployeeDepartments);
        _context.DepartmentManagers.RemoveRange(_context.DepartmentManagers);
        _context.Employees.RemoveRange(_context.Employees);
        _context.Departments.RemoveRange(_context.Departments);
        await _context.SaveChangesAsync();

        var seeder = new HRSeedData(_context, HttpContext.RequestServices.GetRequiredService<ILogger<HRSeedData>>());
        await seeder.SeedAsync();

        _logger.LogInformation("HR tenant database reset and reseeded successfully.");
        return Ok(new { message = "HR tenant database reset and reseeded." });
    }
}
