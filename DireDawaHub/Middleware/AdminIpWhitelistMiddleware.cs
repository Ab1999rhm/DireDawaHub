using DireDawaHub.Data;
using DireDawaHub.Models;
using Microsoft.AspNetCore.Identity;

namespace DireDawaHub.Middleware;

public class AdminIpWhitelistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AdminIpWhitelistMiddleware> _logger;
    private readonly IConfiguration _configuration;

    // Ethiopian IP ranges (CIDR notation)
    private static readonly string[] EthiopianIpRanges = new[]
    {
        "196.188.0.0/16",      // Ethiopian Telecommunications Corporation
        "196.189.0.0/16",      // Ethio Telecom
        "197.156.0.0/16",      // Ethio Telecom
        "197.154.0.0/16",      // Ethio Telecom
        "196.190.0.0/16",      // Ethiopian IPs
        "213.55.0.0/16",       // Ethiopian ISP
        "217.30.0.0/19",       // Ethiopian ISP
        "41.189.0.0/16",       // Ethiopian ISP
        "45.11.0.0/16",        // Ethiopian ISP
        "102.218.0.0/16",      // Ethiopian ISP
        "102.219.0.0/16",      // Ethiopian ISP
        "102.220.0.0/16",      // Ethiopian ISP
        "102.221.0.0/16",      // Ethiopian ISP
        "102.222.0.0/16",      // Ethiopian ISP
        "102.223.0.0/16",      // Ethiopian ISP
    };

    public AdminIpWhitelistMiddleware(RequestDelegate next, ILogger<AdminIpWhitelistMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";
        
        // Only check admin routes
        if (path.StartsWith("/admin"))
        {
            var clientIp = GetClientIpAddress(context);
            var isWhitelisted = IsIpWhitelisted(clientIp);
            var isLocal = IsLocalIp(clientIp);
            
            // Check for bypass key in development/emergencies
            var bypassKey = _configuration["Security:AdminBypassKey"];
            var providedBypassKey = context.Request.Query["bypass_key"].ToString();
            var isBypassValid = !string.IsNullOrEmpty(bypassKey) && bypassKey == providedBypassKey;

            if (!isWhitelisted && !isLocal && !isBypassValid)
            {
                _logger.LogWarning("🚨 BLOCKED ADMIN ACCESS ATTEMPT from IP: {ClientIp} to path: {Path}", clientIp, path);
                
                // Log to security audit
                try
                {
                    var auditLog = new SecurityAuditLog
                    {
                        Action = "Admin_Access_Blocked",
                        Description = $"Unauthorized admin access attempt from IP: {clientIp} to path: {path}",
                        PerformedBy = "SYSTEM",
                        TargetUserId = context.User?.Identity?.Name,
                        Timestamp = DateTime.Now,
                        IpAddress = clientIp,
                        Severity = AuditSeverity.Critical
                    };
                    dbContext.SecurityAuditLogs.Add(auditLog);
                    await dbContext.SaveChangesAsync();
                }
                catch { /* Ignore logging errors */ }

                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Access Denied",
                    message = "Admin panel access is restricted to authorized Ethiopian IP addresses or VPN connections only.",
                    clientIp = clientIp,
                    timestamp = DateTime.UtcNow,
                    support = "Contact system administrator to request IP whitelisting."
                });
                return;
            }

            if (isWhitelisted)
            {
                _logger.LogInformation("✅ Admin access granted from whitelisted IP: {ClientIp}", clientIp);
            }
        }

        await _next(context);
    }

    private string GetClientIpAddress(HttpContext context)
    {
        // Check X-Forwarded-For header (when behind proxy/load balancer)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            // Get the first IP in the chain (original client)
            var ips = forwardedFor.Split(',');
            if (ips.Length > 0)
            {
                return ips[0].Trim();
            }
        }

        // Check X-Real-IP header
        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(realIp))
        {
            return realIp.Trim();
        }

        // Fall back to connection remote IP
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private bool IsLocalIp(string ip)
    {
        if (string.IsNullOrEmpty(ip) || ip == "unknown") return false;
        
        // Check for localhost and private ranges
        return ip == "127.0.0.1" || 
               ip == "::1" || 
               ip.StartsWith("192.168.") || 
               ip.StartsWith("10.") || 
               ip.StartsWith("172.16.") ||
               ip.StartsWith("172.17.") ||
               ip.StartsWith("172.18.") ||
               ip.StartsWith("172.19.") ||
               ip.StartsWith("172.2") ||
               ip.StartsWith("172.30.") ||
               ip.StartsWith("172.31.");
    }

    private bool IsIpWhitelisted(string ip)
    {
        if (string.IsNullOrEmpty(ip) || ip == "unknown") return false;

        // Check configured whitelist from appsettings
        var whitelist = _configuration.GetSection("Security:AdminWhitelist").Get<string[]>();
        if (whitelist != null)
        {
            foreach (var whitelistedIp in whitelist)
            {
                if (ip == whitelistedIp || IsIpInRange(ip, whitelistedIp))
                {
                    return true;
                }
            }
        }

        // Check Ethiopian IP ranges
        foreach (var range in EthiopianIpRanges)
        {
            if (IsIpInRange(ip, range))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsIpInRange(string ip, string cidr)
    {
        try
        {
            if (ip.Contains(':')) return false; // IPv6 not supported in basic implementation
            
            var parts = cidr.Split('/');
            var baseIp = parts[0];
            var prefix = int.Parse(parts[1]);

            var ipParts = ip.Split('.').Select(int.Parse).ToArray();
            var baseIpParts = baseIp.Split('.').Select(int.Parse).ToArray();

            // Convert to 32-bit integers
            uint ipInt = (uint)(ipParts[0] << 24 | ipParts[1] << 16 | ipParts[2] << 8 | ipParts[3]);
            uint baseIpInt = (uint)(baseIpParts[0] << 24 | baseIpParts[1] << 16 | baseIpParts[2] << 8 | baseIpParts[3]);

            // Create mask
            uint mask = prefix == 0 ? 0 : 0xFFFFFFFF << (32 - prefix);

            return (ipInt & mask) == (baseIpInt & mask);
        }
        catch
        {
            return false;
        }
    }
}

// Extension method for easy registration
public static class AdminIpWhitelistMiddlewareExtensions
{
    public static IApplicationBuilder UseAdminIpWhitelist(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AdminIpWhitelistMiddleware>();
    }
}
