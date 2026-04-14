using System;

namespace DireDawaHub.Models;

public class WaterSchedule
{
    public int Id { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = "Scheduled"; // Expected: Scheduled, Active, Delayed
    public string Notes { get; set; } = string.Empty;

    // Additional Tracking Properties
    public string? Zone { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
