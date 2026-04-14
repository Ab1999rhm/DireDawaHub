using System;
using System.ComponentModel.DataAnnotations;

namespace DireDawaHub.Models;

public class SecurityAuditLog
{
    public int Id { get; set; }
    
    [Required]
    public string Action { get; set; } = string.Empty; // e.g., "User_Promoted", "Post_Deleted", "ID_Verified"
    
    public string? Description { get; set; }
    
    [Required]
    public string PerformedBy { get; set; } = string.Empty; // Admin email/username
    
    public string? TargetUserId { get; set; } // If action affects a user
    
    public string? TargetEntityType { get; set; } // e.g., "JobPosting", "WaterSchedule"
    
    public int? TargetEntityId { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.Now;
    
    public string? IpAddress { get; set; }
    
    public string? AdditionalData { get; set; } // JSON string for extra details
    
    public AuditSeverity Severity { get; set; } = AuditSeverity.Info;
}

public enum AuditSeverity
{
    Info,
    Warning,
    Critical
}
