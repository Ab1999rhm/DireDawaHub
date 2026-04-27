using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DireDawaHub.Data;
using DireDawaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace DireDawaHub.Controllers;

[Authorize(Roles = "Contributor")]
public class ContributorController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public ContributorController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> MyPosters()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var posters = await _context.CommunityPosters
            .Where(p => p.ContributorId == userId)
            .OrderByDescending(p => p.PostedDate)
            .ToListAsync();
        return View(posters);
    }

    [HttpGet]
    public IActionResult CreatePoster()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreatePoster(string title, string description, string content, string category, string location, IFormFile image)
    {
        if (image != null && image.Length > 0)
        {
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "posters");
            if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}_{image.FileName}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            var poster = new CommunityPoster
            {
                Title = title,
                Description = description,
                Content = content,
                Category = category,
                Location = location,
                ImagePath = $"/uploads/posters/{fileName}",
                PostedDate = DateTime.Now,
                IsApproved = true, // Auto-approve for now or set to false for moderation
                ContributorId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            };

            _context.CommunityPosters.Add(poster);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> EditPoster(int id)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var poster = await _context.CommunityPosters.FindAsync(id);

        if (poster == null || poster.ContributorId != userId)
        {
            return NotFound();
        }

        return View(poster);
    }

    [HttpPost]
    public async Task<IActionResult> EditPoster(int id, string title, string description, string content, string category, string location, IFormFile? image)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var poster = await _context.CommunityPosters.FindAsync(id);

        if (poster == null || poster.ContributorId != userId)
        {
            return NotFound();
        }

        poster.Title = title;
        poster.Description = description;
        poster.Content = content;
        poster.Category = category;
        poster.Location = location;

        if (image != null && image.Length > 0)
        {
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "posters");
            if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}_{image.FileName}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Optional: delete old image
            var oldPath = Path.Combine(_environment.WebRootPath, poster.ImagePath.TrimStart('/'));
            if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);

            poster.ImagePath = $"/uploads/posters/{fileName}";
        }

        _context.CommunityPosters.Update(poster);
        await _context.SaveChangesAsync();

        return RedirectToAction("MyPosters");
    }

    [HttpPost]
    public async Task<IActionResult> DeletePoster(int id)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var poster = await _context.CommunityPosters.FindAsync(id);

        if (poster != null && poster.ContributorId == userId)
        {
            // Delete file
            var filePath = Path.Combine(_environment.WebRootPath, poster.ImagePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.CommunityPosters.Remove(poster);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("MyPosters");
    }
}
