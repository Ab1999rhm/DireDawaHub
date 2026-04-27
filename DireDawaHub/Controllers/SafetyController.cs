using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DireDawaHub.Data;
using DireDawaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace DireDawaHub.Controllers;

[Authorize(Roles = "Contributor,Admin")]
public class SafetyController : Controller
{
    private readonly ApplicationDbContext _context;

    public SafetyController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var incidents = await _context.PublicSafetyIncidents.OrderByDescending(i => i.ReportedAt).ToListAsync();
        return View(incidents);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(PublicSafetyIncident incident)
    {
        if (ModelState.IsValid)
        {
            _context.PublicSafetyIncidents.Add(incident);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(incident);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var incident = await _context.PublicSafetyIncidents.FindAsync(id);
        if (incident == null) return NotFound();
        return View(incident);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(PublicSafetyIncident incident)
    {
        if (ModelState.IsValid)
        {
            _context.Update(incident);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(incident);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var incident = await _context.PublicSafetyIncidents.FindAsync(id);
        if (incident != null)
        {
            _context.PublicSafetyIncidents.Remove(incident);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
