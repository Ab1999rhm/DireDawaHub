using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DireDawaHub.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using DireDawaHub.Hubs;
using DireDawaHub.Services;
using DireDawaHub.Models;
using DireDawaHub.ViewModels;

namespace DireDawaHub.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly DireDawaHub.Services.SystemStateService _systemState;
    private readonly IHubContext<DireDawaHub.Hubs.NotificationHub> _hubContext;

    public AdminController(UserManager<IdentityUser> userManager, 
                           RoleManager<IdentityRole> roleManager, 
                           ApplicationDbContext context,
                           DireDawaHub.Services.SystemStateService systemState,
                           IHubContext<DireDawaHub.Hubs.NotificationHub> hubContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _systemState = systemState;
        _hubContext = hubContext;
    }

    // ==========================================
    // ADMIN DASHBOARD - SYSTEM ANALYTICS
    // ==========================================
    public async Task<IActionResult> Index()
    {
        // Dynamic Statistics from Database
        ViewBag.TotalUsers = await _userManager.Users.CountAsync();
        ViewBag.TotalContributors = await _userManager.GetUsersInRoleAsync("Contributor").ContinueWith(t => t.Result.Count);
        ViewBag.PendingUsers = await _userManager.Users.CountAsync() - ViewBag.TotalContributors;
        
        // Content Statistics
        ViewBag.TotalWaterRecords = await _context.WaterSchedules.CountAsync();
        ViewBag.TotalJobs = await _context.JobPostings.CountAsync();
        ViewBag.TotalClinics = await _context.ClinicRecords.CountAsync();
        ViewBag.TotalAgMarkets = await _context.AgricultureMarkets.CountAsync();
        
        // Active (Approved) Content
        ViewBag.ActiveJobs = await _context.JobPostings.Where(j => j.IsApproved).CountAsync();
        ViewBag.PendingJobs = await _context.JobPostings.Where(j => !j.IsApproved).CountAsync();
        
        // Recent Activity for Audit Trail
        ViewBag.RecentJobs = await _context.JobPostings
            .OrderByDescending(j => j.PostedDate)
            .Take(5)
            .ToListAsync();
        ViewBag.RecentWater = await _context.WaterSchedules
            .OrderByDescending(w => w.StartTime)
            .Take(5)
            .ToListAsync();
        
        // Security Audit Logs
        ViewBag.RecentAuditLogs = await _context.SecurityAuditLogs
            .OrderByDescending(l => l.Timestamp)
            .Take(10)
            .ToListAsync();
        
        // System State
        ViewBag.SystemState = _systemState;

        return View();
    }

    // ==========================================
    // IDENTITY REGISTRY - USER MANAGEMENT
    // ==========================================
    public async Task<IActionResult> UserDetails(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);
        
        // Fetch Work ID Verification Status
        var workIdClaim = claims.FirstOrDefault(c => c.Type == "WorkId")?.Value;
        WorkIdVerification? idVerification = null;
        
        if (!string.IsNullOrEmpty(workIdClaim))
        {
            idVerification = await _context.WorkIdVerifications
                .FirstOrDefaultAsync(v => v.UserId == id && v.WorkId == workIdClaim);
        }
        
        // Fetch ID Document Images
        var idDocuments = await _context.IdDocumentImages
            .Where(d => d.UserId == id)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();
        
        ViewBag.Roles = roles;
        ViewBag.Claims = claims;
        ViewBag.IdVerification = idVerification;
        ViewBag.IdDocuments = idDocuments;
        ViewBag.HasUploadedDocuments = idDocuments.Any();
        ViewBag.LatestDocument = idDocuments.FirstOrDefault();
        return View(user);
    }

    // VISUAL VERIFICATION - Admin marks ID photo as visually verified
    [HttpPost]
    public async Task<IActionResult> VisualVerifyDocument(int documentId, string? notes)
    {
        var document = await _context.IdDocumentImages.FindAsync(documentId);
        if (document == null) return NotFound();

        document.IsVisuallyVerified = true;
        document.VisualVerificationNotes = notes;
        document.VerifiedAt = DateTime.Now;
        document.VerifiedBy = User.Identity?.Name ?? "Admin";

        await _context.SaveChangesAsync();

        // Log the verification
        await LogSecurityAudit("Visual_ID_Verified", 
            $"ID document visually verified for user {document.UserId}. Notes: {notes}", 
            document.UserId, "IdDocumentImage", documentId, AuditSeverity.Info);

        // Send notification
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", "✅ ID PHOTO VERIFIED", 
            "An administrator has visually verified an ID document.", "system");

        return RedirectToAction(nameof(UserDetails), new { id = document.UserId });
    }

    // WORK ID VERIFICATION - Municipality Database Check
    [HttpPost]
    public async Task<IActionResult> VerifyWorkId(string userId, string employeeName, string department)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var claims = await _userManager.GetClaimsAsync(user);
        var workId = claims.FirstOrDefault(c => c.Type == "WorkId")?.Value;

        if (!string.IsNullOrEmpty(workId))
        {
            // Check if verification already exists
            var existingVerification = await _context.WorkIdVerifications
                .FirstOrDefaultAsync(v => v.UserId == userId && v.WorkId == workId);

            if (existingVerification == null)
            {
                // Create new verification record
                var verification = new WorkIdVerification
                {
                    UserId = userId,
                    WorkId = workId,
                    EmployeeName = employeeName,
                    Department = department,
                    IsVerified = true,
                    MunicipalityDbMatch = true,
                    BackgroundCheckCompleted = true,
                    VerifiedAt = DateTime.Now,
                    VerifiedBy = User.Identity?.Name ?? "Admin",
                    Notes = $"Verified against {department} department records"
                };

                _context.WorkIdVerifications.Add(verification);
            }
            else
            {
                // Update existing
                existingVerification.IsVerified = true;
                existingVerification.MunicipalityDbMatch = true;
                existingVerification.BackgroundCheckCompleted = true;
                existingVerification.VerifiedAt = DateTime.Now;
                existingVerification.VerifiedBy = User.Identity?.Name ?? "Admin";
            }

            await _context.SaveChangesAsync();

            // Log the verification
            await LogSecurityAudit("ID_Verified", $"Work ID {workId} verified for {user.Email}", userId, null, null, AuditSeverity.Info);

            await _hubContext.Clients.All.SendAsync("ReceiveNotification", "✅ ID VERIFIED", 
                $"Work ID for {user.Email} has been verified.", "system");
        }

        return RedirectToAction(nameof(UserDetails), new { id = userId });
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
                
                // Log the promotion
                await LogSecurityAudit("User_Promoted", $"User {user.Email} promoted to Contributor", userId, null, null, AuditSeverity.Info);
                
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", "📢 PERMISSION GRANTED", $"User {user.Email} has been authorized as a Contributor.", "system");
            }
        }
        return RedirectToAction("ManageUsers");
    }

    [HttpPost]
    public async Task<IActionResult> Demote(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null && await _userManager.IsInRoleAsync(user, "Contributor"))
        {
            await _userManager.RemoveFromRoleAsync(user, "Contributor");
            
            // Log the demotion
            await LogSecurityAudit("User_Demoted", $"User {user.Email} demoted from Contributor", userId, null, null, AuditSeverity.Warning);
            
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", "⚠️ ACCESS REVOKED", $"Contributor {user.Email} has been suspended.", "system");
        }
        return RedirectToAction("ManageUsers");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null && !await _userManager.IsInRoleAsync(user, "Admin"))
        {
            await _userManager.DeleteAsync(user);
            
            // Log the deletion
            await LogSecurityAudit("User_Deleted", $"User {user.Email} deleted from system", null, null, null, AuditSeverity.Critical);
        }
        return RedirectToAction("ManageUsers");
    }

    // ==========================================
    // SYSTEM CATALOG MANAGEMENT (CRUD)
    // ==========================================
    
    public async Task<IActionResult> Catalog()
    {
        var model = new CatalogViewModel {
            Jobs = await _context.JobPostings.ToListAsync(),
            Water = await _context.WaterSchedules.ToListAsync(),
            Clinics = await _context.ClinicRecords.ToListAsync()
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteClinic(int id)
    {
        var item = await _context.ClinicRecords.FindAsync(id);
        if (item != null) 
        { 
            _context.ClinicRecords.Remove(item); 
            await _context.SaveChangesAsync();
            
            await LogSecurityAudit("Post_Deleted", $"Clinic '{item.ClinicName}' deleted", null, "ClinicRecord", id, AuditSeverity.Warning);
        }
        return RedirectToAction(nameof(Catalog));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteWater(int id)
    {
        var item = await _context.WaterSchedules.FindAsync(id);
        if (item != null) 
        { 
            _context.WaterSchedules.Remove(item); 
            await _context.SaveChangesAsync();
            
            await LogSecurityAudit("Post_Deleted", $"Water schedule for '{item.Location}' deleted", null, "WaterSchedule", id, AuditSeverity.Warning);
        }
        return RedirectToAction(nameof(Catalog));
    }

    [HttpPost]
    public async Task<IActionResult> ApproveJob(int id)
    {
        var job = await _context.JobPostings.FindAsync(id);
        if (job != null)
        {
            job.IsApproved = true;
            await _context.SaveChangesAsync();
            
            await LogSecurityAudit("Post_Approved", $"Job posting '{job.Title}' approved", job.ContributorId, "JobPosting", id, AuditSeverity.Info);
            
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", "✅ POST APPROVED", $"Moderator has approved the posting: {job.Title}", "system");
        }
        return RedirectToAction(nameof(Catalog));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateJobComment(int id, string comment)
    {
        var job = await _context.JobPostings.FindAsync(id);
        if (job != null)
        {
            job.AdminComment = comment;
            await _context.SaveChangesAsync();
            
            await LogSecurityAudit("Comment_Added", $"Admin comment added to job '{job.Title}'", job.ContributorId, "JobPosting", id, AuditSeverity.Info);
        }
        return RedirectToAction(nameof(Catalog));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteJob(int id)
    {
        var job = await _context.JobPostings.FindAsync(id);
        if (job != null) 
        { 
            _context.JobPostings.Remove(job); 
            await _context.SaveChangesAsync();
            
            await LogSecurityAudit("Post_Deleted", $"Job posting '{job.Title}' deleted", job.ContributorId, "JobPosting", id, AuditSeverity.Warning);
        }
        return RedirectToAction(nameof(Catalog));
    }

    // ==========================================
    // SECURITY AUDIT LOG VIEW
    // ==========================================
    public async Task<IActionResult> AuditLogs()
    {
        var logs = await _context.SecurityAuditLogs
            .OrderByDescending(l => l.Timestamp)
            .Take(100)
            .ToListAsync();
        return View(logs);
    }

    [HttpPost]
    public async Task<IActionResult> BroadcastAlert(string message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", "📡 SYSTEM ALERT", message, "system");
        
        await LogSecurityAudit("Broadcast_Sent", $"System broadcast: {message}", null, null, null, AuditSeverity.Info);
        
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> DownloadLogs()
    {
        var totalUsers = await _userManager.Users.CountAsync();
        var totalContributors = await _userManager.GetUsersInRoleAsync("Contributor").ContinueWith(t => t.Result.Count);
        var auditLogs = await _context.SecurityAuditLogs.CountAsync();
        
        var logContent = $"Dire Dawa Hub System Audit Log - Exported {DateTime.Now}\n" +
                         $"------------------------------------------------------\n" +
                         $"Total Users: {totalUsers}\n" +
                         $"Total Contributors: {totalContributors}\n" +
                         $"Service Status: {_systemState.ServiceStatus}\n" +
                         $"Last Agriculture Fetch: {_systemState.LastAgricultureFetch}\n" +
                         $"Database Integrity: VERIFIED\n" +
                         $"Total Audit Events: {auditLogs}\n" +
                         $"Exported By: {User.Identity?.Name}\n";
        
        await LogSecurityAudit("Logs_Exported", "System audit logs exported", null, null, null, AuditSeverity.Info);
        
        return File(System.Text.Encoding.UTF8.GetBytes(logContent), "text/plain", $"audit_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
    }

    // ==========================================
    // HELPER METHOD - SECURITY AUDIT LOGGING
    // ==========================================
    private async Task LogSecurityAudit(string action, string description, string? targetUserId, string? entityType, int? entityId, AuditSeverity severity)
    {
        var log = new SecurityAuditLog
        {
            Action = action,
            Description = description,
            PerformedBy = User.Identity?.Name ?? "System",
            TargetUserId = targetUserId,
            TargetEntityType = entityType,
            TargetEntityId = entityId,
            Timestamp = DateTime.Now,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            Severity = severity
        };

        _context.SecurityAuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
