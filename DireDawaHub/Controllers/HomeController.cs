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
    private readonly DireDawaHub.Services.WeatherService _weatherService;

    public HomeController(ApplicationDbContext context, DireDawaHub.Services.WeatherService weatherService)
    {
        _context = context;
        _weatherService = weatherService;
    }

    public async Task<IActionResult> Index()
    {
        // Serve the massive "Digital Notice Board" for the community
        var model = new PublicDashboardViewModel();
        
        // Fetch all data live from the database
        model.WaterSchedules = await _context.WaterSchedules.OrderByDescending(w => w.StartTime).ToListAsync();
        model.Clinics = await _context.ClinicRecords.OrderByDescending(c => c.LastUpdated).ToListAsync();
        model.Jobs = await _context.JobPostings
            .Where(j => j.IsApproved)
            .OrderByDescending(j => j.PostedDate)
            .ToListAsync();
        model.AgMarkets = await _context.AgricultureMarkets.OrderByDescending(a => a.RecordedDate).ToListAsync();

        // Fetch active emergency broadcasts that user hasn't acknowledged
        var currentUserId = User.Identity?.IsAuthenticated == true ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value : null;
        
        var broadcasts = await _context.EmergencyBroadcasts
            .Where(b => b.IsActive && (b.ExpiresAt == null || b.ExpiresAt > DateTime.Now))
            .Where(b => !b.Acknowledgments.Any(a => a.UserId == currentUserId))
            .OrderByDescending(b => b.SentAt)
            .Take(5)
            .ToListAsync();

        model.ActiveEmergencyBroadcasts = broadcasts.Where(b => !Request.Cookies.ContainsKey($"AckBroadcast_{b.Id}")).ToList();

        // Fetch Community Posters
        model.Posters = await _context.CommunityPosters
            .Where(p => p.IsApproved)
            .OrderByDescending(p => p.PostedDate)
            .ToListAsync();

        // Fetch Education Announcements
        model.EducationAnnouncements = new List<EducationAnnouncement>
        {
            new EducationAnnouncement { Title = "University Entrance Exam Prep", Description = "Free prep sessions starting next week at the Public Library.", Date = DateTime.Now.AddDays(2) },
            new EducationAnnouncement { Title = "TVET Skills Workshop", Description = "Learn digital masonry and advanced plumbing techniques.", Date = DateTime.Now.AddDays(5) }
        };

        // Fetch Public Safety Incidents
        model.SafetyIncidents = await _context.PublicSafetyIncidents
            .OrderByDescending(i => i.ReportedAt)
            .ToListAsync();

        // Fetch City Statistics
        model.CityStats = await _context.CityStatistics.ToListAsync();

        // Fetch Real Weather Data
        model.Weather = await _weatherService.GetCurrentWeatherAsync();

        return View(model);
    }

    public async Task<IActionResult> PosterDetails(int id)
    {
        var poster = await _context.CommunityPosters.FirstOrDefaultAsync(p => p.Id == id);
        if (poster == null) return NotFound();

        return View(poster);
    }

    [HttpPost]
    public async Task<IActionResult> AcknowledgeBroadcast(int broadcastId)
    {
        Response.Cookies.Append($"AckBroadcast_{broadcastId}", "true", new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });

        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var existingAck = await _context.UserBroadcastAcknowledgments
                    .FirstOrDefaultAsync(a => a.BroadcastId == broadcastId && a.UserId == userId);

                if (existingAck == null)
                {
                    var acknowledgment = new UserBroadcastAcknowledgment
                    {
                        BroadcastId = broadcastId,
                        UserId = userId,
                        AcknowledgedAt = DateTime.Now
                    };

                    _context.UserBroadcastAcknowledgments.Add(acknowledgment);
                    await _context.SaveChangesAsync();
                }
            }
        }

        return Ok(new { success = true });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
