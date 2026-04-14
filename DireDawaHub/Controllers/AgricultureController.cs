using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DireDawaHub.Data;
using DireDawaHub.Models;

namespace DireDawaHub.Controllers;

public class AgricultureController : Controller
{
    private readonly ApplicationDbContext _context;
    public AgricultureController(ApplicationDbContext context) { _context = context; }

    public async Task<IActionResult> Index() { return View(await _context.AgricultureMarkets.OrderByDescending(a => a.RecordedDate).ToListAsync()); }

    [Authorize(Roles = "Admin, Contributor")]
    public IActionResult Create() { return View(new AgricultureMarket { RecordedDate = DateTime.Now }); }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Contributor")]
    public async Task<IActionResult> Create([Bind("Id,CropName,PricePerKg,MarketLocation,RecordedDate,DiseaseAlerts")] AgricultureMarket marketObj)
    {
        if (ModelState.IsValid) { marketObj.RecordedDate = DateTime.Now; _context.Add(marketObj); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        return View(marketObj);
    }
}
