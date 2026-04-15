using System;

namespace DireDawaHub.Models;

public class ClinicRecord 
{
    public int Id { get; set; }
    public string ClinicName { get; set; } = string.Empty;
    public string AvailableDoctors { get; set; } = string.Empty;
    public bool HasEssentialMedicines { get; set; }
    public string EmergencyContact { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
    public string? Location { get; set; }
    public string? Services { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.Now;
    public string? UpdatedBy { get; set; }
}
