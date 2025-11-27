using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;

public interface IApplicantService
{
    Task<ApplicantDto?> GetByIdAsync(Guid id);
    Task<List<ApplicantDto>> GetAllAsync();
    Task<ApplicantDto> CreateAsync(ApplicantCreateDto dto);
}
