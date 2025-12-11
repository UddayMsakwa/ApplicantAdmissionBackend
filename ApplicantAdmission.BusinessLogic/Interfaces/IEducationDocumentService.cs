using ApplicantAdmission.BusinessLogic.Models.Dtos.Education;

namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface IEducationDocumentService
{
    Task<EducationDocumentDto> CreateAsync(EducationDocumentCreateDto dto);
    Task<List<EducationDocumentDto>> GetByApplicantAsync(Guid applicantId);
}
