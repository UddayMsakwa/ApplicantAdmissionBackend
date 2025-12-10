using ApplicantAdmission.BusinessLogic.Models.Dtos.Document;

namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface IDocumentService
{
    Task<DocumentDto> CreateAsync(DocumentCreateDto dto);
    Task<DocumentDto?> GetByIdAsync(Guid id);
    Task<List<DocumentDto>> GetByApplicantAsync(Guid applicantId);
}
