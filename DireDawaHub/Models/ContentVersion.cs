using System;
using System.ComponentModel.DataAnnotations;

namespace DireDawaHub.Models;

public class ContentVersion
{
    public int Id { get; set; }
    
    [Required]
    public string EntityType { get; set; } = string.Empty; // e.g., "JobPosting", "WaterSchedule"
    
    [Required]
    public int EntityId { get; set; }
    
    public string? Title { get; set; }
    
    public string? Content { get; set; }
    
    public string? SerializedData { get; set; } // JSON snapshot of entire object
    
    [Required]
    public string Action { get; set; } = string.Empty; // Created, Updated, Deleted
    
    public string? ChangedBy { get; set; } // User ID who made the change
    
    public string? ChangedByName { get; set; } // Username/email for display
    
    public DateTime ChangedAt { get; set; } = DateTime.Now;
    
    public string? ChangeSummary { get; set; } // Brief description of what changed
    
    public int? PreviousVersionId { get; set; } // Link to previous version for diff
}
