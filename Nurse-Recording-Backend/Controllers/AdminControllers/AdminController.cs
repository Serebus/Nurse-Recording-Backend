using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nurse_Recording_Backend.Data;
using Nurse_Recording_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace Nurse_Recording_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AdminController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("create-admin")]
    [AllowAnonymous]  // One-time use, secure in prod
    public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminModel model)
    {
        if (await AdminExists())
            return BadRequest("Admin already exists. Use login endpoint.");

        var admin = new Nurse
        {
            Username = model.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password, workFactor: _configuration.GetValue<int>("BCrypt:WorkFactor", 11)),
            Email = model.Email,
            IsAuthenticated = true,
            Role = "Admin"
        };

        _context.Nurses.Add(admin);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Admin created successfully", nurseId = admin.Id });
    }

    [HttpGet("check-admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult CheckAdmin()
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        return Ok(new { message = "Admin authorized", username });
    }

    // GET api/admin — returns the current admin's profile
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAdmin()
    {
        var adminIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (adminIdClaim == null || !int.TryParse(adminIdClaim, out int adminId))
            return Unauthorized();

        var admin = await _context.Nurses.FindAsync(adminId);
        if (admin == null)
            return NotFound("Admin not found.");

        return Ok(new
        {
            admin.Id,
            admin.Username,
            admin.Email,
            admin.Role,
            admin.IsAuthenticated
        });
    }

    // PUT api/admin — updates the current admin's username, email, and/or password
    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAdmin([FromBody] UpdateAdminModel model)
    {
        var adminIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (adminIdClaim == null || !int.TryParse(adminIdClaim, out int adminId))
            return Unauthorized();

        var admin = await _context.Nurses.FindAsync(adminId);
        if (admin == null)
            return NotFound("Admin not found.");

        if (!string.IsNullOrWhiteSpace(model.Username))
            admin.Username = model.Username;

        if (!string.IsNullOrWhiteSpace(model.Email))
            admin.Email = model.Email;

        if (!string.IsNullOrWhiteSpace(model.Password))
            admin.Password = BCrypt.Net.BCrypt.HashPassword(
                model.Password,
                workFactor: _configuration.GetValue<int>("BCrypt:WorkFactor", 11));

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Admin updated successfully.",
            admin.Id,
            admin.Username,
            admin.Email,
            admin.Role
        });
    }

    private async Task<bool> AdminExists()
    {
        return await _context.Nurses.AnyAsync(n => !n.Email.StartsWith("aclcnurse") || n.Username == "admin");
    }
}

