using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nurse_Recording_Backend.Controllers;
using Nurse_Recording_Backend.Models;
using NurseRecordingBackend.Tests.Helpers;
using System.Security.Claims;

namespace NurseRecordingBackend.Tests.Controllers;

/// <summary>
/// Unit tests for <see cref="IotAdminController"/>.
/// </summary>
public class IotAdminControllerTests
{
    private const string TestJwtKey = "ThisIsATestJwtKeyThatIsLongEnoughForHmacSha256!";

    private static IConfiguration BuildConfig() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = TestJwtKey,
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience"
            })
            .Build();

    private static async Task<(string email, string plainPassword)> SeedAdmin(
        Nurse_Recording_Backend.Data.AppDbContext ctx)
    {
        const string plainPw = "AdminPass123!";
        var admin = new Nurse
        {
            Username = "iotadmin",
            Email = "admin@test.com",
            Password = BCrypt.Net.BCrypt.HashPassword(plainPw, workFactor: 4),
            Role = "Admin",
            IsAuthenticated = true
        };
        ctx.Nurses.Add(admin);
        await ctx.SaveChangesAsync();
        return (admin.Email, plainPw);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenAdminCredentialsAreValid()
    {
        using var context = TestDbHelper.CreateContext();
        var (email, password) = await SeedAdmin(context);
        var controller = new IotAdminController(context, BuildConfig());

        var result = await controller.Login(new LoginModel { Email = email, Password = password });

        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = okResult.Value!;
        Assert.NotNull(value.GetType().GetProperty("accessToken")?.GetValue(value));
    }

    [Fact]
    public async Task Login_TokenContains_IotAdminRoleClaim()
    {
        using var context = TestDbHelper.CreateContext();
        var (email, password) = await SeedAdmin(context);
        var controller = new IotAdminController(context, BuildConfig());

        var result = await controller.Login(new LoginModel { Email = email, Password = password });

        var okResult = Assert.IsType<OkObjectResult>(result);
        var tokenStr = okResult.Value!.GetType().GetProperty("accessToken")?.GetValue(okResult.Value) as string;

        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(tokenStr);
        var roleClaim = jwt.Claims.FirstOrDefault(c => 
            c.Type == ClaimTypes.Role || c.Type == "role" || c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

        Assert.NotNull(roleClaim);
        Assert.Equal("IotAdmin", roleClaim!.Value);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenUserIsNotAdmin()
    {
        using var context = TestDbHelper.CreateContext();
        // Seed a regular Nurse
        var nurse = new Nurse
        {
            Username = "regular",
            Email = "nurse@test.com",
            Password = BCrypt.Net.BCrypt.HashPassword("password", workFactor: 4),
            Role = "Nurse",
            IsAuthenticated = true
        };
        context.Nurses.Add(nurse);
        await context.SaveChangesAsync();

        var controller = new IotAdminController(context, BuildConfig());

        var result = await controller.Login(new LoginModel { Email = nurse.Email, Password = "password" });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenPasswordIsIncorrect()
    {
        using var context = TestDbHelper.CreateContext();
        var (email, _) = await SeedAdmin(context);
        var controller = new IotAdminController(context, BuildConfig());

        var result = await controller.Login(new LoginModel { Email = email, Password = "WrongPassword" });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
