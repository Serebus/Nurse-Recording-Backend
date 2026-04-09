using System.ComponentModel.DataAnnotations;

namespace Nurse_Recording_Backend.Models;

public class Followup
{
    [Key]
    public int Id { get; set; }
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public DateTime Date { get; set; }
    public string Notes { get; set; } = string.Empty;
    // Add more fields as needed
}
