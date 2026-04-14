using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DireDawaHub.ViewModels;
using DireDawaHub.Data;
using DireDawaHub.Models;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IO;
using System;
using System.Linq;

namespace DireDawaHub.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _context = context;
        _environment = environment;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                // 2. Redirect Admin directly to Admin Core
                if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                
                // 3. Redirect Verified Contributors to their dedicated workspace
                if (user != null && await _userManager.IsInRoleAsync(user, "Contributor"))
                {
                    return RedirectToAction("Index", "Contributor");
                }
                
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, model.Password);
            
            if (result.Succeeded)
            {
                // Inject the custom Work ID as an Identity Claim
                await _userManager.AddClaimAsync(user, new Claim("WorkId", model.WorkId));
                
                // Create upload directory if it doesn't exist
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "id_documents");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Save ID Document Photo
                if (model.IdDocumentPhoto != null && model.IdDocumentPhoto.Length > 0)
                {
                    var fileName = $"{user.Id}_{DateTime.Now:yyyyMMddHHmmss}_{model.IdDocumentPhoto.FileName}";
                    var filePath = Path.Combine(uploadsPath, fileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.IdDocumentPhoto.CopyToAsync(stream);
                    }

                    // Save to database
                    var idDocument = new IdDocumentImage
                    {
                        UserId = user.Id,
                        FileName = model.IdDocumentPhoto.FileName,
                        FilePath = $"/uploads/id_documents/{fileName}",
                        FileSize = model.IdDocumentPhoto.Length,
                        ContentType = model.IdDocumentPhoto.ContentType,
                        DocumentType = model.DocumentType,
                        IsVisuallyVerified = false,
                        UploadedAt = DateTime.Now
                    };

                    // Save Selfie if provided
                    if (model.SelfieWithId != null && model.SelfieWithId.Length > 0)
                    {
                        var selfieFileName = $"{user.Id}_selfie_{DateTime.Now:yyyyMMddHHmmss}_{model.SelfieWithId.FileName}";
                        var selfieFilePath = Path.Combine(uploadsPath, selfieFileName);
                        
                        using (var stream = new FileStream(selfieFilePath, FileMode.Create))
                        {
                            await model.SelfieWithId.CopyToAsync(stream);
                        }

                        idDocument.SelfieFilePath = $"/uploads/id_documents/{selfieFileName}";
                    }

                    _context.IdDocumentImages.Add(idDocument);
                    await _context.SaveChangesAsync();
                }
                
                // Immediately log them in, they will be given pure civilian access until Admin approves
                await _signInManager.SignInAsync(user, isPersistent: false);
                
                // Show success message about pending verification
                TempData["SuccessMessage"] = "Registration successful! Your ID document has been submitted for verification. An administrator will review it shortly.";
                
                return RedirectToAction("Index", "Home");
            }
            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
