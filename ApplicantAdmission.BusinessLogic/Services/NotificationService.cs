using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.BusinessLogic.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicantDbContext _db;

    public NotificationService(ApplicantDbContext db)
    {
        _db = db;
    }

    public async Task NotifyApplicantAsync(Guid applicantId, string message)
    {
        var email = await _db.Applicants
            .Where(a => a.Id == applicantId)
            .Join(_db.Users, a => a.UserId, u => u.Id, (a, u) => u.Email)
            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(email)) return;

        _db.Notifications.Add(new NotificationEntity
        {
            Id = Guid.NewGuid(),
            ToEmail = email,
            Subject = "Applicant notification",
            Body = message,
            Status = "Queued",
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }

    public async Task NotifyManagerAsync(Guid managerId, string message)
    {
        var email = await _db.Managers
            .Where(m => m.Id == managerId)
            .Select(m => m.Email)
            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(email)) return;

        _db.Notifications.Add(new NotificationEntity
        {
            Id = Guid.NewGuid(),
            ToEmail = email,
            Subject = "Manager notification",
            Body = message,
            Status = "Queued",
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }
}
