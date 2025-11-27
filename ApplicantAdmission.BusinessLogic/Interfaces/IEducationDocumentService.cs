using ApplicantAdmission.BusinessLogic.Models.Dtos.Education;

public interface IEducationDocumentService
{
    Task<List<EducationDocumentDto>> GetByApplicantAsync(Guid applicantId);
}
