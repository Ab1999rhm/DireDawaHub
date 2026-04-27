using System.Collections.Generic;
using DireDawaHub.Models;

namespace DireDawaHub.ViewModels;

public class PublicDashboardViewModel
{
    public IEnumerable<WaterSchedule> WaterSchedules { get; set; } = new List<WaterSchedule>();
    public IEnumerable<ClinicRecord> Clinics { get; set; } = new List<ClinicRecord>();
    public IEnumerable<JobPosting> Jobs { get; set; } = new List<JobPosting>();
    public IEnumerable<AgricultureMarket> AgMarkets { get; set; } = new List<AgricultureMarket>();
    public IEnumerable<EmergencyBroadcast> ActiveEmergencyBroadcasts { get; set; } = new List<EmergencyBroadcast>();
    public IEnumerable<CommunityPoster> Posters { get; set; } = new List<CommunityPoster>();
    
    // New sections for the enhanced dashboard
    public IEnumerable<EducationAnnouncement> EducationAnnouncements { get; set; } = new List<EducationAnnouncement>();
    public IEnumerable<PublicSafetyIncident> SafetyIncidents { get; set; } = new List<PublicSafetyIncident>();
    public IEnumerable<CityStatistic> CityStats { get; set; } = new List<CityStatistic>();
    public WeatherData Weather { get; set; } = new WeatherData();
}

public class EducationAnnouncement
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = "📚";
    public DateTime Date { get; set; } = DateTime.Now;
}

public class WeatherData
{
    public double Temperature { get; set; } = 28.5;
    public string Condition { get; set; } = "Sunny";
    public string Icon { get; set; } = "🌤️";
    public double Humidity { get; set; } = 45;
    public double WindSpeed { get; set; } = 12.4;
    public string Forecast { get; set; } = "Clear skies expected for the next 24 hours.";
}
