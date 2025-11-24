using ApplicantAdmission.BusinessLogic.Models.Applicant;

namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface IApplicantService
{
    Task<ApplicantDto?> GetByIdAsync(Guid id);
    Task<ApplicantDto> CreateAsync(ApplicantCreateDto dto);
    Task<List<ApplicantDto>> GetAllAsync();
}
