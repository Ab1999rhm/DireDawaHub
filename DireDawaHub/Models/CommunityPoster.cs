using System;
using System.ComponentModel.DataAnnotations;

namespace DireDawaHub.Models;

public class CommunityPoster
{
    public int Id { get; set; }
    
    [Required]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public string? Content { get; set; }
    
    [Required]
    public string ImagePath { get; set; } = string.Empty;
    
    public DateTime PostedDate { get; set; } = DateTime.Now;
    
    public bool IsApproved { get; set; } = false;
    
    public string? ContributorId { get; set; }
    
    public string? Category { get; set; } // e.g., "Event", "Announcement", "Education"
    
    public string? Location { get; set; }
}
