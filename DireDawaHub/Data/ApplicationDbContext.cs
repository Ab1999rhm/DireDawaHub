using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DireDawaHub.Models;

namespace DireDawaHub.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<WaterSchedule> WaterSchedules { get; set; }
    public DbSet<ClinicRecord> ClinicRecords { get; set; }
    public DbSet<JobPosting> JobPostings { get; set; }
    public DbSet<AgricultureMarket> AgricultureMarkets { get; set; }
    public DbSet<WorkIdVerification> WorkIdVerifications { get; set; }
    public DbSet<SecurityAuditLog> SecurityAuditLogs { get; set; }
    public DbSet<IdDocumentImage> IdDocumentImages { get; set; }
    public DbSet<ContentVersion> ContentVersions { get; set; }
    public DbSet<EmergencyBroadcast> EmergencyBroadcasts { get; set; }
    public DbSet<UserBroadcastAcknowledgment> UserBroadcastAcknowledgments { get; set; }
    public DbSet<CommunityPoster> CommunityPosters { get; set; }
}
