using DireDawaHub.Data;
using DireDawaHub.Models;
using DireDawaHub.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DireDawaHub.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Serve the massive "Digital Notice Board" for the community
        var model = new PublicDashboardViewModel();
        
        // Fetch all data live from the database
        model.WaterSchedules = await _context.WaterSchedules.OrderByDescending(w => w.StartTime).Take(6).ToListAsync();
        model.Clinics = await _context.ClinicRecords.OrderByDescending(c => c.LastUpdated).ToListAsync();
        model.Jobs = await _context.JobPostings.OrderByDescending(j => j.PostedDate).Take(8).ToListAsync();
        model.AgMarkets = await _context.AgricultureMarkets.OrderByDescending(a => a.RecordedDate).Take(6).ToListAsync();

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
