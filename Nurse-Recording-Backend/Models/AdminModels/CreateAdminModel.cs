namespace Nurse_Recording_Backend.Models;

public class CreateAdminModel
{
    public string Username { get; set; } = "admin";
    public string Password { get; set; } = string.Empty;
    public string Email    { get; set; } = string.Empty;
}
