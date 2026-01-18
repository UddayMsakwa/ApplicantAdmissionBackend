using ApplicantAdmission.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.Jobs;

public class Worker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _config;

    public Worker(IServiceScopeFactory scopeFactory, ILogger<Worker> logger, IConfiguration config)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalSeconds = _config.GetValue("Jobs:NotificationSenderIntervalSeconds", 30);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SendQueuedNotifications(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Notification sender job failed");
            }

            await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
        }
    }

    private async Task SendQueuedNotifications(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicantDbContext>();

        var queued = await db.Notifications
            .Where(n => n.Status == "Queued")
            .OrderBy(n => n.CreatedAt)
            .Take(20)
            .ToListAsync(ct);

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
            }
        }

        await db.SaveChangesAsync(ct);
    }
}
