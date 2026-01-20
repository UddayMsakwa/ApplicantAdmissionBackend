using System;
using System.Threading.Tasks;

namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface INotificationService
{
    Task NotifyApplicantAsync(Guid applicantId, string message);

    
    Task NotifyStaffAsync(Guid staffUserId, string message);

    
    Task NotifyEmailAsync(string toEmail, string subject, string body);
}
