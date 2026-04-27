using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DireDawaHub.Data;
using DireDawaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace DireDawaHub.Controllers;

[Authorize(Roles = "Contributor,Admin")]
public class StatsController : Controller
{
    private readonly ApplicationDbContext _context;

    public StatsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var stats = await _context.CityStatistics.ToListAsync();
        return View(stats);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(CityStatistic stat)
    {
        if (ModelState.IsValid)
        {
            _context.CityStatistics.Add(stat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(stat);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var stat = await _context.CityStatistics.FindAsync(id);
        if (stat == null) return NotFound();
        return View(stat);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CityStatistic stat)
    {
        if (ModelState.IsValid)
        {
            _context.Update(stat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(stat);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var stat = await _context.CityStatistics.FindAsync(id);
        if (stat != null)
        {
            _context.CityStatistics.Remove(stat);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
