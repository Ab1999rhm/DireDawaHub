using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DireDawaHub.Data;
using DireDawaHub.Models;

namespace DireDawaHub.Controllers;

public class JobsController : Controller
{
    private readonly ApplicationDbContext _context;
    public JobsController(ApplicationDbContext context) { _context = context; }

    public async Task<IActionResult> Index() { return View(await _context.JobPostings.OrderByDescending(j => j.PostedDate).ToListAsync()); }

    [Authorize(Roles = "Admin, Contributor")]
    public IActionResult Create() { return View(new JobPosting { PostedDate = DateTime.Now }); }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Contributor")]
    public async Task<IActionResult> Create([Bind("Id,Title,Company,Location,Description,IsTrainingOpportunity")] JobPosting jobPosting)
    {
        if (ModelState.IsValid) 
        { 
            jobPosting.PostedDate = DateTime.Now; 
            jobPosting.IsApproved = User.IsInRole("Admin"); // Admins are auto-approved
            jobPosting.ContributorId = User.Identity?.Name;
            
            _context.Add(jobPosting); 
            await _context.SaveChangesAsync(); 
            return RedirectToAction(nameof(Index)); 
        }
        return View(jobPosting);
    }

    [Authorize(Roles = "Admin, Contributor")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.JobPostings.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Contributor")]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Company,Location,Description,IsTrainingOpportunity,PostedDate,ContributorId,IsApproved")] JobPosting jobPosting)
    {
        if (id != jobPosting.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(jobPosting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(jobPosting);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Contributor")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.JobPostings.FindAsync(id);
        if (item != null)
        {
            _context.JobPostings.Remove(item);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
