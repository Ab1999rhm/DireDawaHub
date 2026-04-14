using System.Collections.Generic;
using DireDawaHub.Models;

namespace DireDawaHub.ViewModels;

public class PublicDashboardViewModel
{
    public IEnumerable<WaterSchedule> WaterSchedules { get; set; } = new List<WaterSchedule>();
    public IEnumerable<ClinicRecord> Clinics { get; set; } = new List<ClinicRecord>();
    public IEnumerable<JobPosting> Jobs { get; set; } = new List<JobPosting>();
    public IEnumerable<AgricultureMarket> AgMarkets { get; set; } = new List<AgricultureMarket>();
}
