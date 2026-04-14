using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DireDawaHub.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
    {
        try
        {
            var smtpHost = _configuration["Smtp:Host"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["Smtp:Port"] ?? "587");
            var smtpUsername = _configuration["Smtp:Username"] ?? "";
            var smtpPassword = _configuration["Smtp:Password"] ?? "";
            var fromEmail = _configuration["Smtp:FromEmail"] ?? "noreply@diredawahub.et";
            var fromName = _configuration["Smtp:FromName"] ?? "Dire Dawa Hub";

            if (string.IsNullOrWhiteSpace(smtpUsername) || string.IsNullOrWhiteSpace(smtpPassword))
            {
                _logger.LogWarning("SMTP credentials not configured. Email not sent to {Email}", toEmail);
                return false;
            }

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword)
            };

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            message.To.Add(toEmail);

            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            return false;
        }
    }

    // Notification Templates
    public async Task<bool> SendContributorApprovedNotificationAsync(string toEmail, string userName)
    {
        var subject = "✅ Your Contributor Account Has Been Approved - Dire Dawa Hub";
        var body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
            <h2 style='color: #10b981;'>🎉 Congratulations, {userName}!</h2>
            <p>Your application to become a verified contributor has been <strong>approved</strong>.</p>
            <p>You now have full access to:</p>
            <ul>
                <li>Post job opportunities</li>
                <li>Update water distribution schedules</li>
                <li>Manage clinic information</li>
                <li>Share agricultural market updates</li>
            </ul>
            <p><a href='https://diredawahub.et/Contributor' style='background: #3b82f6; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; display: inline-block;'>Access Your Dashboard</a></p>
            <hr style='margin: 30px 0; border: none; border-top: 1px solid #e5e7eb;'>
            <p style='color: #6b7280; font-size: 14px;'>Dire Dawa Hub - Community Driven Data Platform</p>
        </div>";

        return await SendEmailAsync(toEmail, subject, body);
    }

    public async Task<bool> SendContributorRejectedNotificationAsync(string toEmail, string userName, string? reason = null)
    {
        var subject = "⚠️ Contributor Application Status - Dire Dawa Hub";
        var body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
            <h2 style='color: #f59e0b;'>Hello {userName},</h2>
            <p>Your contributor privileges have been temporarily suspended or your application was not approved.</p>
            {(string.IsNullOrWhiteSpace(reason) ? "" : $"<p><strong>Reason:</strong> {reason}</p>")}
            <p>If you believe this was done in error, please contact the administrator.</p>
            <hr style='margin: 30px 0; border: none; border-top: 1px solid #e5e7eb;'>
            <p style='color: #6b7280; font-size: 14px;'>Dire Dawa Hub - Community Driven Data Platform</p>
        </div>";

        return await SendEmailAsync(toEmail, subject, body);
    }

    public async Task<bool> SendJobApprovedNotificationAsync(string toEmail, string jobTitle)
    {
        var subject = $"✅ Your Job Posting '{jobTitle}' is Now Live";
        var body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
            <h2 style='color: #10b981;'>Job Posting Approved!</h2>
            <p>Your job posting <strong>""{jobTitle}""</strong> has been reviewed and is now live on the Dire Dawa Hub.</p>
            <p><a href='https://diredawahub.et/Jobs' style='background: #3b82f6; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; display: inline-block;'>View Job Board</a></p>
            <hr style='margin: 30px 0; border: none; border-top: 1px solid #e5e7eb;'>
            <p style='color: #6b7280; font-size: 14px;'>Dire Dawa Hub - Community Driven Data Platform</p>
        </div>";

        return await SendEmailAsync(toEmail, subject, body);
    }

    public async Task<bool> SendJobRejectedNotificationAsync(string toEmail, string jobTitle, string? adminComment = null)
    {
        var subject = $"⚠️ Job Posting '{jobTitle}' Requires Changes";
        var body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
            <h2 style='color: #f59e0b;'>Job Posting Not Approved</h2>
            <p>Your job posting <strong>""{jobTitle}""</strong> was not approved at this time.</p>
            {(string.IsNullOrWhiteSpace(adminComment) ? "" : $"<p><strong>Admin Feedback:</strong> {adminComment}</p>")}
            <p>You can edit and resubmit this posting from your contributor dashboard.</p>
            <p><a href='https://diredawahub.et/Contributor' style='background: #3b82f6; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; display: inline-block;'>Edit Posting</a></p>
            <hr style='margin: 30px 0; border: none; border-top: 1px solid #e5e7eb;'>
            <p style='color: #6b7280; font-size: 14px;'>Dire Dawa Hub - Community Driven Data Platform</p>
        </div>";

        return await SendEmailAsync(toEmail, subject, body);
    }

    public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
    {
        var subject = "🌍 Welcome to Dire Dawa Hub";
        var body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
            <h2 style='color: #3b82f6;'>Welcome, {userName}!</h2>
            <p>Thank you for joining the Dire Dawa Hub community platform.</p>
            <p>Your account is currently pending approval. An administrator will review your information and verify your work ID.</p>
            <p>Once approved, you'll be able to:</p>
            <ul>
                <li>Post job opportunities</li>
                <li>Update water schedules</li>
                <li>Manage health information</li>
                <li>Share market updates</li>
            </ul>
            <p>We'll notify you via email when your account is approved.</p>
            <hr style='margin: 30px 0; border: none; border-top: 1px solid #e5e7eb;'>
            <p style='color: #6b7280; font-size: 14px;'>Dire Dawa Hub - Community Driven Data Platform</p>
        </div>";

        return await SendEmailAsync(toEmail, subject, body);
    }
}
