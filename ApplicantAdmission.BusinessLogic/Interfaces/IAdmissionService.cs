using ApplicantAdmission.BusinessLogic.Models.Admission;

namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface IAdmissionService
{
    Task<ApplicantAdmissionDto?> GetByIdAsync(Guid id);
    Task<List<ApplicantAdmissionDto>> GetByApplicantAsync(Guid applicantId);
}
