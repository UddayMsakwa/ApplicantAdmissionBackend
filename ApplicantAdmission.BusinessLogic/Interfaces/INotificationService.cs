using System;
using System.Threading.Tasks;

namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface INotificationService
{
    Task NotifyApplicantAsync(Guid applicantId, string message);
    Task NotifyManagerAsync(Guid managerId, string message);
}
