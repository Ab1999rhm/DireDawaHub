using DireDawaHub.Data;
using DireDawaHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DireDawaHub.Services;

public class VersionTrackingService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<VersionTrackingService> _logger;

    public VersionTrackingService(ApplicationDbContext context, ILogger<VersionTrackingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task TrackCreationAsync<T>(T entity, string entityType, int entityId, string changedBy, string changedByName) where T : class
    {
        var version = new ContentVersion
        {
            EntityType = entityType,
            EntityId = entityId,
            Title = GetEntityTitle(entity),
            Content = GetEntityContent(entity),
            SerializedData = JsonSerializer.Serialize(entity),
            Action = "Created",
            ChangedBy = changedBy,
            ChangedByName = changedByName,
            ChangedAt = DateTime.Now,
            ChangeSummary = $"{entityType} created by {changedByName}"
        };

        _context.ContentVersions.Add(version);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Tracked creation of {entityType} #{entityId}");
    }

    public async Task TrackUpdateAsync<T>(T oldEntity, T newEntity, string entityType, int entityId, string changedBy, string changedByName) where T : class
    {
        var changes = GetChanges(oldEntity, newEntity);
        
        var version = new ContentVersion
        {
            EntityType = entityType,
            EntityId = entityId,
            Title = GetEntityTitle(newEntity),
            Content = GetEntityContent(newEntity),
            SerializedData = JsonSerializer.Serialize(newEntity),
            Action = "Updated",
            ChangedBy = changedBy,
            ChangedByName = changedByName,
            ChangedAt = DateTime.Now,
            ChangeSummary = string.IsNullOrWhiteSpace(changes) ? $"{entityType} updated by {changedByName}" : changes
        };

        // Link to previous version
        var previousVersion = await _context.ContentVersions
            .Where(v => v.EntityType == entityType && v.EntityId == entityId)
            .OrderByDescending(v => v.ChangedAt)
            .FirstOrDefaultAsync();
        
        if (previousVersion != null)
        {
            version.PreviousVersionId = previousVersion.Id;
        }

        _context.ContentVersions.Add(version);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Tracked update of {entityType} #{entityId}");
    }

    public async Task TrackDeletionAsync(string entityType, int entityId, string title, string changedBy, string changedByName)
    {
        var version = new ContentVersion
        {
            EntityType = entityType,
            EntityId = entityId,
            Title = title,
            Action = "Deleted",
            ChangedBy = changedBy,
            ChangedByName = changedByName,
            ChangedAt = DateTime.Now,
            ChangeSummary = $"{entityType} '{title}' deleted by {changedByName}"
        };

        _context.ContentVersions.Add(version);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Tracked deletion of {entityType} #{entityId}");
    }

    public async Task<List<ContentVersion>> GetVersionHistoryAsync(string entityType, int entityId)
    {
        return await _context.ContentVersions
            .Where(v => v.EntityType == entityType && v.EntityId == entityId)
            .OrderByDescending(v => v.ChangedAt)
            .ToListAsync();
    }

    public async Task<ContentVersion?> GetVersionAsync(int versionId)
    {
        return await _context.ContentVersions.FindAsync(versionId);
    }

    private string? GetEntityTitle<T>(T entity) where T : class
    {
        // Try to get Title or Name property
        var titleProp = entity.GetType().GetProperty("Title") 
            ?? entity.GetType().GetProperty("Name")
            ?? entity.GetType().GetProperty("ClinicName")
            ?? entity.GetType().GetProperty("Location");
        
        return titleProp?.GetValue(entity)?.ToString();
    }

    private string? GetEntityContent<T>(T entity) where T : class
    {
        // Try to get Description or Content property
        var contentProp = entity.GetType().GetProperty("Description") 
            ?? entity.GetType().GetProperty("Content")
            ?? entity.GetType().GetProperty("Notes");
        
        return contentProp?.GetValue(entity)?.ToString();
    }

    private string GetChanges<T>(T oldEntity, T newEntity) where T : class
    {
        var changes = new List<string>();
        var properties = typeof(T).GetProperties()
            .Where(p => p.CanRead && p.CanWrite && !p.PropertyType.IsClass);

        foreach (var prop in properties)
        {
            var oldValue = prop.GetValue(oldEntity)?.ToString();
            var newValue = prop.GetValue(newEntity)?.ToString();
            
            if (oldValue != newValue)
            {
                changes.Add($"{prop.Name}: '{oldValue}' → '{newValue}'");
            }
        }

        return string.Join("; ", changes);
    }
}
