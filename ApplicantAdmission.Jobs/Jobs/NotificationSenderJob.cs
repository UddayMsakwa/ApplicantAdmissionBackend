using ApplicantAdmission.DataAccess;
using Microsoft.EntityFrameworkCore;
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

        var queued = await db.Notifications
            .Where(n => n.Status == "Queued")
            .OrderBy(n => n.CreatedAt)
            .Take(20)
            .ToListAsync();

        if (queued.Count == 0) return;

        foreach (var n in queued)
        {
            try
            {
                _logger.LogInformation("SENDING EMAIL To={to} Subject={subject}", n.ToEmail, n.Subject);

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
