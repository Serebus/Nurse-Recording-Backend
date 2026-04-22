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
[Route("api/iotadmin")]
public class IotAdminController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public IotAdminController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var identifier = !string.IsNullOrEmpty(model.Email) ? model.Email : model.Username;

        var admin = await _context.Nurses.FirstOrDefaultAsync(n => (n.Email == identifier || n.Username == identifier) && n.Role == "Admin");

        if (admin == null || !BCrypt.Net.BCrypt.Verify(model.Password, admin.Password))
            return Unauthorized("Invalid admin credentials");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
            new Claim(ClaimTypes.Email, admin.Email),
            new Claim(ClaimTypes.Name, admin.Username),
            new Claim("nurseId", admin.Id.ToString()),
            new Claim(ClaimTypes.Role, "IotAdmin")
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

        return Ok(new
        {
            accessToken = jwt,
            token = jwt,
            user = new
            {
                admin,
                isAuthenticated = true,
                adminDetails = new { adminId = admin.Id }
            }
        });
    }
}
