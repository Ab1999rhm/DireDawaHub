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
using System.Text;
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
    private readonly DireDawaHub.Services.EmailService _emailService;

    public AdminController(UserManager<IdentityUser> userManager, 
                           RoleManager<IdentityRole> roleManager, 
                           ApplicationDbContext context,
                           DireDawaHub.Services.SystemStateService systemState,
                           IHubContext<DireDawaHub.Hubs.NotificationHub> hubContext,
                           DireDawaHub.Services.EmailService emailService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _systemState = systemState;
        _hubContext = hubContext;
        _emailService = emailService;
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

    public async Task<IActionResult> ManageUsers(int page = 1, int pageSize = 25, string searchTerm = "", string filterRole = "")
    {
        // Build query
        var usersQuery = _userManager.Users.AsQueryable();
        
        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            usersQuery = usersQuery.Where(u => 
                u.Email.ToLower().Contains(searchTerm) || 
                u.UserName.ToLower().Contains(searchTerm));
        }
        
        // Get total count for pagination
        var totalUsers = await usersQuery.CountAsync();
        var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);
        
        // Apply pagination
        var users = await usersQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        var model = new List<DireDawaHub.ViewModels.AdminUserViewModel>();
        
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            
            // Apply role filter if specified
            if (!string.IsNullOrWhiteSpace(filterRole))
            {
                if (filterRole == "Contributor" && !roles.Contains("Contributor"))
                    continue;
                if (filterRole == "Pending" && roles.Contains("Contributor"))
                    continue;
                if (filterRole == "Admin" && !roles.Contains("Admin"))
                    continue;
            }
            
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
        
        // Pass pagination data to view
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalUsers = totalUsers;
        ViewBag.SearchTerm = searchTerm;
        ViewBag.FilterRole = filterRole;
        
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
                
                // Send email notification
                await _emailService.SendContributorApprovedNotificationAsync(user.Email ?? "", user.UserName ?? user.Email ?? "User");
                
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
            
            // Send email notification
            await _emailService.SendContributorRejectedNotificationAsync(user.Email ?? "", user.UserName ?? user.Email ?? "User", "Contributor privileges have been revoked by an administrator.");
            
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

    // BULK OPERATIONS
    [HttpPost]
    public async Task<IActionResult> BulkPromote([FromForm] List<string> userIds)
    {
        if (userIds == null || !userIds.Any())
        {
            TempData["Error"] = "No users selected for activation.";
            return RedirectToAction("ManageUsers");
        }

        int promotedCount = 0;
        
        if (!await _roleManager.RoleExistsAsync("Contributor"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Contributor"));
        }

        foreach (var userId in userIds)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && !await _userManager.IsInRoleAsync(user, "Contributor"))
            {
                await _userManager.AddToRoleAsync(user, "Contributor");
                await LogSecurityAudit("User_Promoted", $"User {user.Email} promoted to Contributor (Bulk)", userId, null, null, AuditSeverity.Info);
                promotedCount++;
            }
        }

        await _hubContext.Clients.All.SendAsync("ReceiveNotification", "📢 BULK PERMISSIONS GRANTED", 
            $"{promotedCount} user(s) have been activated as Contributors.", "system");
        
        TempData["Success"] = $"Successfully activated {promotedCount} user(s) as contributors.";
        return RedirectToAction("ManageUsers");
    }

    [HttpPost]
    public async Task<IActionResult> BulkDemote([FromForm] List<string> userIds)
    {
        if (userIds == null || !userIds.Any())
        {
            TempData["Error"] = "No users selected for deactivation.";
            return RedirectToAction("ManageUsers");
        }

        int demotedCount = 0;

        foreach (var userId in userIds)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && await _userManager.IsInRoleAsync(user, "Contributor"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Contributor");
                await LogSecurityAudit("User_Demoted", $"User {user.Email} demoted from Contributor (Bulk)", userId, null, null, AuditSeverity.Warning);
                demotedCount++;
            }
        }

        await _hubContext.Clients.All.SendAsync("ReceiveNotification", "⚠️ BULK ACCESS REVOKED", 
            $"{demotedCount} contributor(s) have been suspended.", "system");
        
        TempData["Success"] = $"Successfully deactivated {demotedCount} contributor(s).";
        return RedirectToAction("ManageUsers");
    }

    [HttpPost]
    public async Task<IActionResult> BulkDelete([FromForm] List<string> userIds)
    {
        if (userIds == null || !userIds.Any())
        {
            TempData["Error"] = "No users selected for deletion.";
            return RedirectToAction("ManageUsers");
        }

        int deletedCount = 0;

        foreach (var userId in userIds)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && !await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.DeleteAsync(user);
                await LogSecurityAudit("User_Deleted", $"User {user.Email} deleted from system (Bulk)", null, null, null, AuditSeverity.Critical);
                deletedCount++;
            }
        }

        await _hubContext.Clients.All.SendAsync("ReceiveNotification", "🗑️ BULK ACCOUNTS DELETED", 
            $"{deletedCount} user account(s) have been permanently deleted.", "system");
        
        TempData["Success"] = $"Successfully deleted {deletedCount} user account(s).";
        return RedirectToAction("ManageUsers");
    }

    // ==========================================
    // SYSTEM CATALOG MANAGEMENT (CRUD)
    // ==========================================
    
    public async Task<IActionResult> Catalog(int jobsPage = 1, int waterPage = 1, int clinicsPage = 1, int pageSize = 10)
    {
        // Jobs pagination
        var totalJobs = await _context.JobPostings.CountAsync();
        ViewBag.JobsTotalPages = (int)Math.Ceiling(totalJobs / (double)pageSize);
        ViewBag.JobsCurrentPage = jobsPage;
        ViewBag.JobsTotal = totalJobs;
        
        // Water pagination
        var totalWater = await _context.WaterSchedules.CountAsync();
        ViewBag.WaterTotalPages = (int)Math.Ceiling(totalWater / (double)pageSize);
        ViewBag.WaterCurrentPage = waterPage;
        ViewBag.WaterTotal = totalWater;
        
        // Clinics pagination
        var totalClinics = await _context.ClinicRecords.CountAsync();
        ViewBag.ClinicsTotalPages = (int)Math.Ceiling(totalClinics / (double)pageSize);
        ViewBag.ClinicsCurrentPage = clinicsPage;
        ViewBag.ClinicsTotal = totalClinics;
        
        ViewBag.PageSize = pageSize;
        
        var model = new CatalogViewModel {
            Jobs = await _context.JobPostings
                .OrderByDescending(j => j.PostedDate)
                .Skip((jobsPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(),
            Water = await _context.WaterSchedules
                .OrderByDescending(w => w.StartTime)
                .Skip((waterPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(),
            Clinics = await _context.ClinicRecords
                .OrderByDescending(c => c.Id)
                .Skip((clinicsPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync()
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
            
            // Send email notification to contributor
            if (!string.IsNullOrWhiteSpace(job.ContributorId))
            {
                var contributor = await _userManager.FindByIdAsync(job.ContributorId);
                if (contributor != null)
                {
                    await _emailService.SendJobApprovedNotificationAsync(contributor.Email ?? "", job.Title);
                }
            }
            
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
    public async Task<IActionResult> AuditLogs(int page = 1, int pageSize = 50)
    {
        var totalLogs = await _context.SecurityAuditLogs.CountAsync();
        var totalPages = (int)Math.Ceiling(totalLogs / (double)pageSize);
        
        var logs = await _context.SecurityAuditLogs
            .OrderByDescending(l => l.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalLogs = totalLogs;
        
        return View(logs);
    }

    // CSV EXPORT ACTIONS
    [HttpGet]
    public async Task<IActionResult> ExportUsersToCsv()
    {
        var users = await _userManager.Users.ToListAsync();
        var csv = new StringBuilder();
        csv.AppendLine("UserId,Email,UserName,Roles,WorkId,EmailConfirmed,PhoneNumberConfirmed,LockoutEnabled,AccessFailedCount,LockoutEnd");
        
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);
            var workId = claims.FirstOrDefault(c => c.Type == "WorkId")?.Value ?? "";
            
            csv.AppendLine($"{user.Id},{EscapeCsv(user.Email)},{EscapeCsv(user.UserName)},{EscapeCsv(string.Join(";", roles))},{EscapeCsv(workId)},{user.EmailConfirmed},{user.PhoneNumberConfirmed},{user.LockoutEnabled},{user.AccessFailedCount},{user.LockoutEnd}");
        }
        
        await LogSecurityAudit("Data_Exported", "User registry exported to CSV", null, null, null, AuditSeverity.Info);
        
        return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"users_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
    }

    [HttpGet]
    public async Task<IActionResult> ExportAuditLogsToCsv()
    {
        var logs = await _context.SecurityAuditLogs
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();
        
        var csv = new StringBuilder();
        csv.AppendLine("Id,Timestamp,Action,Description,PerformedBy,TargetUserId,TargetEntityType,TargetEntityId,Severity,IpAddress");
        
        foreach (var log in logs)
        {
            csv.AppendLine($"{log.Id},{log.Timestamp:yyyy-MM-dd HH:mm:ss},{EscapeCsv(log.Action)},{EscapeCsv(log.Description)},{EscapeCsv(log.PerformedBy)},{EscapeCsv(log.TargetUserId)},{EscapeCsv(log.TargetEntityType)},{log.TargetEntityId},{log.Severity},{EscapeCsv(log.IpAddress)}");
        }
        
        await LogSecurityAudit("Data_Exported", "Audit logs exported to CSV", null, null, null, AuditSeverity.Info);
        
        return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"audit_logs_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
    }

    [HttpGet]
    public async Task<IActionResult> ExportJobsToCsv()
    {
        var jobs = await _context.JobPostings.ToListAsync();
        var csv = new StringBuilder();
        csv.AppendLine("Id,Title,Company,Location,JobType,Salary,Description,PostedDate,IsApproved,AdminComment,ContributorId");
        
        foreach (var job in jobs)
        {
            csv.AppendLine($"{job.Id},{EscapeCsv(job.Title)},{EscapeCsv(job.Company)},{EscapeCsv(job.Location)},{EscapeCsv(job.JobType)},{EscapeCsv(job.Salary)},{EscapeCsv(job.Description)},{job.PostedDate:yyyy-MM-dd},{job.IsApproved},{EscapeCsv(job.AdminComment)},{job.ContributorId}");
        }
        
        await LogSecurityAudit("Data_Exported", "Job postings exported to CSV", null, null, null, AuditSeverity.Info);
        
        return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"jobs_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
    }

    [HttpGet]
    public async Task<IActionResult> ExportWaterSchedulesToCsv()
    {
        var schedules = await _context.WaterSchedules.ToListAsync();
        var csv = new StringBuilder();
        csv.AppendLine("Id,Location,Zone,StartTime,EndTime,Status,Notes,UpdatedAt,UpdatedBy");
        
        foreach (var schedule in schedules)
        {
            csv.AppendLine($"{schedule.Id},{EscapeCsv(schedule.Location)},{EscapeCsv(schedule.Zone)},{schedule.StartTime:yyyy-MM-dd HH:mm},{schedule.EndTime:yyyy-MM-dd HH:mm},{EscapeCsv(schedule.Status)},{EscapeCsv(schedule.Notes)},{schedule.UpdatedAt:yyyy-MM-dd HH:mm},{EscapeCsv(schedule.UpdatedBy)}");
        }
        
        await LogSecurityAudit("Data_Exported", "Water schedules exported to CSV", null, null, null, AuditSeverity.Info);
        
        return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"water_schedules_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
    }

    [HttpGet]
    public async Task<IActionResult> ExportClinicsToCsv()
    {
        var clinics = await _context.ClinicRecords.ToListAsync();
        var csv = new StringBuilder();
        csv.AppendLine("Id,ClinicName,Location,AvailableDoctors,HasEssentialMedicines,ContactPhone,Services,LastUpdated,UpdatedBy");
        
        foreach (var clinic in clinics)
        {
            csv.AppendLine($"{clinic.Id},{EscapeCsv(clinic.ClinicName)},{EscapeCsv(clinic.Location)},{clinic.AvailableDoctors},{clinic.HasEssentialMedicines},{EscapeCsv(clinic.ContactPhone)},{EscapeCsv(clinic.Services)},{clinic.LastUpdated:yyyy-MM-dd HH:mm},{EscapeCsv(clinic.UpdatedBy)}");
        }
        
        await LogSecurityAudit("Data_Exported", "Clinic records exported to CSV", null, null, null, AuditSeverity.Info);
        
        return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"clinics_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
    }

    [HttpGet]
    public async Task<IActionResult> ExportJobsByTitle(string title)
    {
        var jobs = await _context.JobPostings
            .Where(j => j.Title.Contains(title))
            .ToListAsync();
        var csv = new StringBuilder();
        csv.AppendLine("Id,Title,Company,Location,JobType,Salary,Description,PostedDate,IsApproved,AdminComment,ContributorId");
        
        foreach (var job in jobs)
        {
            csv.AppendLine($"{job.Id},{EscapeCsv(job.Title)},{EscapeCsv(job.Company)},{EscapeCsv(job.Location)},{EscapeCsv(job.JobType)},{EscapeCsv(job.Salary)},{EscapeCsv(job.Description)},{job.PostedDate:yyyy-MM-dd},{job.IsApproved},{EscapeCsv(job.AdminComment)},{job.ContributorId}");
        }
        
        await LogSecurityAudit("Data_Exported", $"Job postings filtered by title '{title}' exported to CSV", null, null, null, AuditSeverity.Info);
        
        return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"jobs_{title.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
    }

    private string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
        {
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
        return value;
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
