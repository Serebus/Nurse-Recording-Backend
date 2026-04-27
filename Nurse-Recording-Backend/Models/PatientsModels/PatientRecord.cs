using System.ComponentModel.DataAnnotations;

namespace Nurse_Recording_Backend.Models;

public class PatientRecord
{
    [Key]
    public int Id { get; set; }
    public string RecordId { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public DateTime Date { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string Symptom { get; set; } = string.Empty;
    public string Treatment { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string Status { get; set; } = "Open";
}
