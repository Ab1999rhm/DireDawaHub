using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DireDawaHub.Data;
using DireDawaHub.Models;

namespace DireDawaHub.Controllers;

public class HealthController : Controller
{
    private readonly ApplicationDbContext _context;
    public HealthController(ApplicationDbContext context) { _context = context; }

    public async Task<IActionResult> Index() { return View(await _context.ClinicRecords.OrderByDescending(c => c.LastUpdated).ToListAsync()); }

    [Authorize(Roles = "Admin, Contributor")]
    public IActionResult Create() { return View(new ClinicRecord { LastUpdated = DateTime.Now }); }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Contributor")]
    public async Task<IActionResult> Create([Bind("Id,ClinicName,AvailableDoctors,HasEssentialMedicines,EmergencyContact,LastUpdated")] ClinicRecord clinicRecord)
    {
        if (ModelState.IsValid) { clinicRecord.LastUpdated = DateTime.Now; _context.Add(clinicRecord); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        return View(clinicRecord);
    }

    [Authorize(Roles = "Admin, Contributor")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.ClinicRecords.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Contributor")]
    public async Task<IActionResult> Edit(int id, [Bind("Id,ClinicName,AvailableDoctors,HasEssentialMedicines,EmergencyContact,LastUpdated")] ClinicRecord clinicRecord)
    {
        if (id != clinicRecord.Id) return NotFound();
        if (ModelState.IsValid)
        {
            clinicRecord.LastUpdated = DateTime.Now;
            _context.Update(clinicRecord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(clinicRecord);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Contributor")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.ClinicRecords.FindAsync(id);
        if (item != null)
        {
            _context.ClinicRecords.Remove(item);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
