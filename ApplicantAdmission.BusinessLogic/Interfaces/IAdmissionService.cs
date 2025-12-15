using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;
using ApplicantAdmission.BusinessLogic.Models.Pagination;
using ApplicantAdmission.DataAccess.Enums;

namespace ApplicantAdmission.BusinessLogic.Interfaces
{
    public interface IAdmissionService
    {
        Task<ApplicantAdmissionDto?> GetByIdAsync(Guid id);
        Task<List<ApplicantAdmissionDto>> GetByApplicantAsync(Guid applicantId);
        Task<List<ApplicantAdmissionDto>> GetByManagerAsync(Guid managerId); 
        Task<List<ApplicantAdmissionDto>> GetAllAsync();                     

        Task<ApplicantAdmissionDto> CreateAsync(ApplicantAdmissionCreateDto dto);
        Task<ApplicantAdmissionDto> AssignManagerAsync(Guid admissionId, Guid managerId);
        Task<ApplicantAdmissionDto> UpdateStatusAsync(Guid admissionId, AdmissionStatus status);
        Task<PagedResult<ApplicantAdmissionDto>> GetPagedAsync(int page, int pageSize);
    }
}



