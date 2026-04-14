using System;
using System.ComponentModel.DataAnnotations;

namespace DireDawaHub.Models;

public class WorkIdVerification
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    public string WorkId { get; set; } = string.Empty;
    
    public bool IsVerified { get; set; } = false;
    
    public string? EmployeeName { get; set; }
    
    public string? Department { get; set; }
    
    public bool BackgroundCheckCompleted { get; set; } = false;
    
    public DateTime VerifiedAt { get; set; }
    
    public string? VerifiedBy { get; set; }
    
    public string? Notes { get; set; }
    
    // Municipality database validation
    public bool MunicipalityDbMatch { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
