using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Nurse_Recording_Backend.Data;
using Nurse_Recording_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Nurse_Recording_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var nurse = await _context.Nurses.FirstOrDefaultAsync(n => n.Email == model.Email);
        if (nurse == null || nurse.Password != model.Password) // Plain text - hash in prod
            return Unauthorized("Invalid credentials");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, nurse.Id.ToString()),
            new Claim(ClaimTypes.Email, nurse.Email),
            new Claim(ClaimTypes.Name, nurse.Username),
            new Claim("nurseId", nurse.Id.ToString()),
            new Claim(ClaimTypes.Role, nurse.Role ?? "Nurse")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { token, user = new { nurse, isAuthenticated = true, nurseDetails = new { nurseId = nurse.Id } } });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Client clears token, server no blacklist for simplicity
        return Ok(new { message = "Logged out" });
    }
}

public class LoginModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
