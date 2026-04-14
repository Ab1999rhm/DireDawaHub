using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using DireDawaHub.Data;
using DireDawaHub.Models;
using DireDawaHub.Hubs;

namespace DireDawaHub.Controllers;

public class WaterController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;

    public WaterController(ApplicationDbContext context, IHubContext<NotificationHub> hubContext) 
    { 
        _context = context; 
        _hubContext = hubContext;
    }

    public async Task<IActionResult> Index() { return View(await _context.WaterSchedules.OrderByDescending(w => w.StartTime).ToListAsync()); }

    [Authorize(Roles = "Admin, Contributor")]
    public IActionResult Create() { return View(new WaterSchedule { StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(4) }); }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Contributor")]
    public async Task<IActionResult> Create([Bind("Id,Location,StartTime,EndTime,Status,Notes")] WaterSchedule waterSchedule)
    {
        if (ModelState.IsValid) 
        { 
            _context.Add(waterSchedule); 
            await _context.SaveChangesAsync(); 
            
            // SIGNALR NOTIFICATION PUSH
            // When the database saves successfully, force-push this event to all active browsers instantly!
            var alertMessage = string.IsNullOrEmpty(waterSchedule.Notes) ? $"Status changed to {waterSchedule.Status}" : waterSchedule.Notes;
            await _hubContext.Clients.All.SendAsync("ReceiveWaterAlert", waterSchedule.Location, waterSchedule.Status, alertMessage);

            return RedirectToAction(nameof(Index)); 
        }
        return View(waterSchedule);
    }
}
