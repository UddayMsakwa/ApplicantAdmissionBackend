using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;

namespace ApplicantAdmission.BusinessLogic.Interfaces
{
    public interface IAdmissionService
    {
        Task<ApplicantAdmissionDto?> GetByIdAsync(Guid id);
        Task<List<ApplicantAdmissionDto>> GetByApplicantAsync(Guid applicantId);
        Task<ApplicantAdmissionDto> CreateAsync(ApplicantAdmissionCreateDto dto);
        Task<ApplicantAdmissionDto> AssignManagerAsync(Guid admissionId, Guid managerId);
        Task<ApplicantAdmissionDto> UpdateStatusAsync(Guid admissionId, string status);
    }
}

