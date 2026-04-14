using System;

namespace DireDawaHub.Models;

public class JobPosting
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PostedDate { get; set; } = DateTime.Now;
    public bool IsTrainingOpportunity { get; set; }
    
    // Additional Properties for Export/Details
    public string? JobType { get; set; } // e.g., Full-time, Part-time
    public string? Salary { get; set; }
    
    // Moderation Features
    public bool IsApproved { get; set; } = false; // Default to pending
    public string? AdminComment { get; set; }
    public string? ContributorId { get; set; } // Owner of the post
    
    // Content Scheduling
    public DateTime? ScheduledPublishAt { get; set; } // When to auto-publish
    public bool IsScheduled => ScheduledPublishAt.HasValue && ScheduledPublishAt > DateTime.Now;
    public bool IsPublished => IsApproved && (!ScheduledPublishAt.HasValue || ScheduledPublishAt <= DateTime.Now);
}
