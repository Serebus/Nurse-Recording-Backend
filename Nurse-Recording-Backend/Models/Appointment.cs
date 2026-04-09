using System.ComponentModel.DataAnnotations;

namespace Nurse_Recording_Backend.Models;

public class Appointment
{
    [Key]
    public int Id { get; set; }
    public string AppointmentId { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public DateTime Date { get; set; }
    public string Time { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
