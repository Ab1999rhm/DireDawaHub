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
    public async Task<IActionResult> Create([Bind("Id,Title,Company,Location,Description,PostedDate,IsTrainingOpportunity")] JobPosting jobPosting)
    {
        if (ModelState.IsValid) { jobPosting.PostedDate = DateTime.Now; _context.Add(jobPosting); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        return View(jobPosting);
    }
}
