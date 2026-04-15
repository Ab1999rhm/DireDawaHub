namespace DireDawaHub.Models;

public class EmergencyBroadcast
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.Now;
    public DateTime? ExpiresAt { get; set; }
    public string SentBy { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string Severity { get; set; } = "Critical"; // Critical, Warning, Info
    public ICollection<UserBroadcastAcknowledgment> Acknowledgments { get; set; } = new List<UserBroadcastAcknowledgment>();
}

public class UserBroadcastAcknowledgment
{
    public int Id { get; set; }
    public int BroadcastId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime AcknowledgedAt { get; set; } = DateTime.Now;
    public EmergencyBroadcast Broadcast { get; set; } = null!;
}
