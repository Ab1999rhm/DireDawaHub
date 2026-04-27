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

    [Authorize(Roles = "Admin, Contributor")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.AgricultureMarkets.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Contributor")]
    public async Task<IActionResult> Edit(int id, [Bind("Id,CropName,PricePerKg,MarketLocation,RecordedDate,DiseaseAlerts")] AgricultureMarket marketObj)
    {
        if (id != marketObj.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(marketObj);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(marketObj);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Contributor")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.AgricultureMarkets.FindAsync(id);
        if (item != null)
        {
            _context.AgricultureMarkets.Remove(item);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
