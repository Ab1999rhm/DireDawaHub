using System;
using System.ComponentModel.DataAnnotations;

namespace DireDawaHub.Models;

public class PublicSafetyIncident
{
    public int Id { get; set; }
    
    [Required]
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public string IncidentType { get; set; } = "General"; // Traffic, Fire, Security
    
    public string Status { get; set; } = "Active"; // Active, Resolved, Alert
    
    public string Location { get; set; } = string.Empty;
    
    public DateTime ReportedAt { get; set; } = DateTime.Now;
    
    public string? ContactInfo { get; set; }
}

public class CityStatistic
{
    public int Id { get; set; }
    
    [Required]
    public string Label { get; set; } = string.Empty;
    
    public string Value { get; set; } = string.Empty;
    
    public string Icon { get; set; } = "📊";
    
    public string Category { get; set; } = "General"; // Population, Infrastructure, Economy
}
