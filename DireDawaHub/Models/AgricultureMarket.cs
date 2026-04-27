using System;

namespace DireDawaHub.Models;

public class AgricultureMarket
{
    public int Id { get; set; }
    public string CropName { get; set; } = string.Empty;
    public decimal PricePerKg { get; set; }
    public string MarketLocation { get; set; } = string.Empty;
    public DateTime RecordedDate { get; set; } = DateTime.Now;
    public string DiseaseAlerts { get; set; } = string.Empty;
    public string? ProductImagePath { get; set; }
}
