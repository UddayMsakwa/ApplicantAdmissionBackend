using System;
using System.Threading.Tasks;
using ApplicantAdmission.BusinessLogic.Interfaces;

namespace ApplicantAdmission.BusinessLogic.Services;

public class NotificationService : INotificationService
{
    public Task NotifyApplicantAsync(Guid applicantId, string message)
    {
        Console.WriteLine($"[APPLICANT {applicantId}] {message}");
        return Task.CompletedTask;
    }

    public Task NotifyManagerAsync(Guid managerId, string message)
    {
        Console.WriteLine($"[MANAGER {managerId}] {message}");
        return Task.CompletedTask;
    }
}
