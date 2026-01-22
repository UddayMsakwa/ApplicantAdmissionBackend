
using System.Net;
using System.Net.Mail;
using ApplicantAdmission.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;

public sealed class NotificationSenderJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationSenderJob> _logger;

    public NotificationSenderJob(IServiceScopeFactory scopeFactory, ILogger<NotificationSenderJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using var scope = _scopeFactory.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<ApplicantDbContext>();
        var email = scope.ServiceProvider.GetRequiredService<IOptions<EmailSettings>>().Value;

        var queued = await db.Notifications
            .Where(n => n.Status == "Queued")
            .OrderBy(n => n.CreatedAt)
            .Take(50)
            .ToListAsync();

        if (queued.Count == 0) return;

        using var client = new SmtpClient(email.Host, email.Port)
        {
            EnableSsl = email.EnableSsl
        };

        if (!string.IsNullOrWhiteSpace(email.Username))
            client.Credentials = new NetworkCredential(email.Username, email.Password);

        foreach (var n in queued)
        {
            try
            {
                using var msg = new MailMessage
                {
                    From = new MailAddress(email.FromEmail, email.FromName),
                    Subject = n.Subject,
                    Body = n.Body,
                    IsBodyHtml = false
                };
                msg.To.Add(new MailAddress(n.ToEmail));

                _logger.LogInformation("SMTP SEND To={to} Subject={subject}", n.ToEmail, n.Subject);

                await client.SendMailAsync(msg);

                n.Status = "Sent";
                n.SentAt = DateTime.UtcNow;
                n.Error = null;
            }
            catch (Exception ex)
            {
                n.Status = "Failed";
                n.Error = ex.Message;
                _logger.LogError(ex, "Notification send failed. Id={id}", n.Id);
            }
        }

        await db.SaveChangesAsync();
    }
}

public sealed class EmailSettings
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 1025;
    public bool EnableSsl { get; set; } = false;

    public string FromEmail { get; set; } = "no-reply@applicantadmission.local";
    public string FromName { get; set; } = "Applicant Admission";

    public string? Username { get; set; }
    public string? Password { get; set; }
}
