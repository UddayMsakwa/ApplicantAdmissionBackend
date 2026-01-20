using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;

namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface IApplicantService
{
    Task<ApplicantDto?> GetByIdAsync(Guid id);
    Task<List<ApplicantDto>> GetAllAsync();

    
    Task<ApplicantDto> GetMeAsync(Guid userId);
    Task<ApplicantDto> UpdateMeAsync(Guid userId, ApplicantUpdateDto dto);
}
