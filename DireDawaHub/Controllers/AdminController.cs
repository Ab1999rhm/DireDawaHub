using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DireDawaHub.Data;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;

namespace DireDawaHub.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        // System Analytics
        ViewBag.TotalUsers = await _userManager.Users.CountAsync();
        ViewBag.TotalWaterRecords = await _context.WaterSchedules.CountAsync();
        ViewBag.TotalJobs = await _context.JobPostings.CountAsync();
        
        // System Audit Log Simulation (Recent Actions)
        var recentJobs = await _context.JobPostings
            .OrderByDescending(j => j.PostedDate)
            .Take(4)
            .ToListAsync();
        ViewBag.RecentJobs = recentJobs;

        return View();
    }

    public async Task<IActionResult> ManageUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        var model = new List<DireDawaHub.ViewModels.AdminUserViewModel>();
        
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);
            var workId = claims.FirstOrDefault(c => c.Type == "WorkId")?.Value ?? "N/A";

            model.Add(new DireDawaHub.ViewModels.AdminUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Roles = roles,
                WorkId = workId
            });
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Promote(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            if (!await _roleManager.RoleExistsAsync("Contributor"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Contributor"));
            }
            
            if (!await _userManager.IsInRoleAsync(user, "Contributor"))
            {
                await _userManager.AddToRoleAsync(user, "Contributor");
            }
        }
        return RedirectToAction("ManageUsers");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteJob(int id)
    {
        var job = await _context.JobPostings.FindAsync(id);
        if (job != null)
        {
            _context.JobPostings.Remove(job);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
