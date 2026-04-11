using System.ComponentModel.DataAnnotations;

namespace Nurse_Recording_Backend.Models;

public class Followup
{
    [Key]
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int RecordId { get; set; }
    public Patient Patient { get; set; } = null!;
    public DateTime Date { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string New_Diagnostic { get; set; } = string.Empty;
    public string Symptom { get; set; } = string.Empty;
    public string New_Symptom { get; set; } = string.Empty;
    public string Treatment { get; set; } = string.Empty;
    public string Additional_Treatment { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
