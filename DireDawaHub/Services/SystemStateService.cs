namespace DireDawaHub.Services;

public class SystemStateService
{
    public DateTime LastAgricultureFetch { get; set; } = DateTime.Now.AddHours(-1);
    public string ServiceStatus { get; set; } = "Operational";
    public int TotalSystemErrors { get; set; } = 0;
    public double DatabaseLatency { get; set; } = 12.4;
}
