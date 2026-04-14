using DireDawaHub.Models;

namespace DireDawaHub.ViewModels;

public class CatalogViewModel
{
    public List<JobPosting> Jobs { get; set; } = new();
    public List<WaterSchedule> Water { get; set; } = new();
    public List<ClinicRecord> Clinics { get; set; } = new();
}
