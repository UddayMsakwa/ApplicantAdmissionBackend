
namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface IAdmissionLockService
{
    
    Task EnsureApplicantNotClosedAsync(Guid applicantId);

    
    Task<Guid> GetLatestAdmissionIdOrThrowAsync(Guid applicantId);
}
