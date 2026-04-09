using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nurse_Recording_Backend.Data;
using Nurse_Recording_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Nurse_Recording_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
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
            Password = model.Password, // Hash in prod: BCrypt.Net or AspNetCore.Identity
            Email = model.Email,
            IsAuthenticated = true
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

    private async Task<bool> AdminExists()
    {
        return await _context.Nurses.AnyAsync(n => !n.Email.StartsWith("aclcnurse") || n.Username == "admin");
    }
}

public class CreateAdminModel
{
    public string Username { get; set; } = "admin";
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
