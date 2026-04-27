using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nurse_Recording_Backend.Models;

public class Alarm
{
    public int Id { get; set; }
    
    [Required]
    public AlarmState State { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [Required]
    public string DeviceId { get; set; } = string.Empty;
    
    public int? NurseId { get; set; }
    
    [ForeignKey("NurseId")]
    public Nurse? Nurse { get; set; }

    [ForeignKey("DeviceId")]
    public Device? Device { get; set; }
}
