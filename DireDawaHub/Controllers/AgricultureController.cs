using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DireDawaHub.Data;
using DireDawaHub.Models;

namespace DireDawaHub.Controllers;

public class AgricultureController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public AgricultureController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index() { return View(await _context.AgricultureMarkets.OrderByDescending(a => a.RecordedDate).ToListAsync()); }

    [Authorize(Roles = "Admin, Contributor")]
    public IActionResult Create() { return View(new AgricultureMarket { RecordedDate = DateTime.Now }); }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Contributor")]
    public async Task<IActionResult> Create([Bind("Id,CropName,PricePerKg,MarketLocation,RecordedDate,DiseaseAlerts")] AgricultureMarket marketObj, IFormFile productImage)
    {
        if (productImage == null || productImage.Length == 0)
        {
            ModelState.AddModelError("ProductImagePath", "A product image is required.");
        }

        if (ModelState.IsValid)
        {
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "agriculture");
            if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}_{productImage.FileName}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await productImage.CopyToAsync(stream);
            }

            marketObj.ProductImagePath = $"/uploads/agriculture/{fileName}";
            marketObj.RecordedDate = DateTime.Now;
            _context.Add(marketObj);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
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
    public async Task<IActionResult> Edit(int id, [Bind("Id,CropName,PricePerKg,MarketLocation,RecordedDate,DiseaseAlerts,ProductImagePath")] AgricultureMarket marketObj, IFormFile? productImage)
    {
        if (id != marketObj.Id) return NotFound();

        if (ModelState.IsValid)
        {
            if (productImage != null && productImage.Length > 0)
            {
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "agriculture");
                if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}_{productImage.FileName}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await productImage.CopyToAsync(stream);
                }

                // Delete old image if it exists
                if (!string.IsNullOrEmpty(marketObj.ProductImagePath))
                {
                    var oldPath = Path.Combine(_environment.WebRootPath, marketObj.ProductImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                marketObj.ProductImagePath = $"/uploads/agriculture/{fileName}";
            }

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
