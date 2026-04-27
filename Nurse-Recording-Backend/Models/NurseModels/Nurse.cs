using System.ComponentModel.DataAnnotations;

namespace Nurse_Recording_Backend.Models;

public class Nurse
{
    [Key]
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; // Hash in practice
    public string Email { get; set; } = string.Empty;
    public bool IsAuthenticated { get; set; } = true;
    public string Role { get; set; } = "Nurse"; // Admin/Nurse
}
