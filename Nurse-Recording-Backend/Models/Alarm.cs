using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nurse_Recording_Backend.Models;

public class Alarm
{
    public int Id { get; set; }
    
    [Required]
    public AlarmState State { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public string? DeviceId { get; set; }
    
    public int? NurseId { get; set; }
    
    [ForeignKey("NurseId")]
    public Nurse? Nurse { get; set; }
}
