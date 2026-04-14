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
}
