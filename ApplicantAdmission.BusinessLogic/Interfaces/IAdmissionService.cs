using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;
using ApplicantAdmission.BusinessLogic.Models.Pagination;
using ApplicantAdmission.DataAccess.Enums;
using ApplicantAdmission.DataAccess.Entities;



namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface IAdmissionService
{
    Task<ApplicantAdmissionDto?> GetByIdAsync(Guid id);

    
    Task<PagedResult<ApplicantAdmissionDto>> GetMyPagedAsync(Guid userId, int page, int pageSize);
    Task<List<ApplicantAdmissionDto>> GetMyAsync(Guid userId);
    Task<ApplicantAdmissionDto> CreateMyAsync(Guid userId, ApplicantAdmissionCreateDto dto);

    
    Task<List<ApplicantAdmissionDto>> GetByApplicantAsync(Guid applicantId);
    Task<List<ApplicantAdmissionDto>> GetByManagerAsync(Guid managerUserId);
    Task<PagedResult<ApplicantAdmissionDto>> GetPagedAsync(int page, int pageSize);
    Task<ApplicantAdmissionEntity> GetEntityForOwnershipCheckAsync(Guid admissionId);
    Task AssignManagerAsync(Guid admissionId, Guid managerUserId);
    Task<ApplicantAdmissionDto> UpdateStatusAsync(Guid admissionId, AdmissionStatus status);
}
