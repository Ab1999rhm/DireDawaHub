using Microsoft.EntityFrameworkCore;
using DireDawaHub.Data;
using DireDawaHub.Models;

namespace DireDawaHub.Services;

public class ContentSchedulerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ContentSchedulerService> _logger;

    public ContentSchedulerService(IServiceProvider serviceProvider, ILogger<ContentSchedulerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Content Scheduler Service is starting...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAndPublishScheduledContentAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Content Scheduler Service");
            }

            // Check every minute
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task CheckAndPublishScheduledContentAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Find scheduled job postings that should be published now
        var scheduledJobs = await context.JobPostings
            .Where(j => j.ScheduledPublishAt.HasValue 
                && j.ScheduledPublishAt <= DateTime.Now 
                && !j.IsApproved)
            .ToListAsync();

        if (scheduledJobs.Any())
        {
            _logger.LogInformation($"Publishing {scheduledJobs.Count} scheduled job postings");
            
            foreach (var job in scheduledJobs)
            {
                job.IsApproved = true;
                _logger.LogInformation($"Auto-published job: {job.Title}");
            }

            await context.SaveChangesAsync();
        }
    }
}
