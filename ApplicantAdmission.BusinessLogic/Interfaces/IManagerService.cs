using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Manager;
using ApplicantAdmission.BusinessLogic.Models.Pagination;

namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface IManagerService
{
    Task<List<ManagerDto>> GetAllAsync();

    Task<PagedResult<ApplicantAdmissionDto>> GetApplicationsAsync(
        string? status, int page, int pageSize);

    Task<ApplicantDto> GetApplicantAsync(Guid applicantId);

    Task<ApplicantAdmissionDto> TakeAdmissionAsync(Guid admissionId, Guid managerId);
    Task<ApplicantAdmissionDto> ReleaseAdmissionAsync(Guid admissionId, Guid managerId);

    Task<ApplicantDto> UpdateApplicantAsync(Guid applicantId, ApplicantUpdateDto dto);
}
