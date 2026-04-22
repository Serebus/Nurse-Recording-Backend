namespace Nurse_Recording_Backend.Models;

public class LoginModel
{
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string Password { get; set; } = string.Empty;
}
