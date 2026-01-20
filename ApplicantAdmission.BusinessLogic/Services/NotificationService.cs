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

        await NotifyEmailAsync(email, "Applicant notification", message);
    }

    public async Task NotifyStaffAsync(Guid staffUserId, string message)
    {
        var staff = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == staffUserId);

        if (staff == null) return;
        if (string.IsNullOrWhiteSpace(staff.Email)) return;

        await NotifyEmailAsync(staff.Email, "Staff notification", message);
    }

    public async Task NotifyEmailAsync(string toEmail, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(toEmail)) return;

        _db.Notifications.Add(new NotificationEntity
        {
            Id = Guid.NewGuid(),
            ToEmail = toEmail,
            Subject = subject,
            Body = body,
            Status = "Queued",
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }
}
