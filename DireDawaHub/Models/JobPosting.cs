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
}
