using System.ComponentModel.DataAnnotations;

namespace Nurse_Recording_Backend.Models;

public class Patient
{
    [Key]
    public int Id { get; set; }
    public string Firstname { get; set; } = string.Empty;
    public string Middlename { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Facebook { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EmergencyContact { get; set; } = string.Empty;
}
