using System.ComponentModel.DataAnnotations;

namespace Nurse_Recording_Backend.Models;

public class Device
{
    [Key]
    public string DeviceId { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    public ICollection<Alarm> Alarms { get; set; } = new List<Alarm>();
}
