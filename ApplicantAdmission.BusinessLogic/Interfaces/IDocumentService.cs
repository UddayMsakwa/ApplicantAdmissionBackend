using ApplicantAdmission.BusinessLogic.Models.Dtos.Document;

public interface IDocumentService
{
    Task<List<DocumentDto>> GetByApplicantAsync(Guid applicantId);
}
