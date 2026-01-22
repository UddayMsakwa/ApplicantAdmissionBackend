using ApplicantAdmission.BusinessLogic.Models.Dtos.Documents;
using Microsoft.AspNetCore.Http;

namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface IApplicantDocumentsService
{
    Task<PassportDocumentDto> GetPassportAsync(Guid applicantId);
    Task<PassportDocumentDto> UpsertPassportAsync(Guid applicantId, PassportDocumentUpdateDto dto);

    Task<EducationDocumentDto> GetEducationAsync(Guid applicantId);
    Task<EducationDocumentDto> UpsertEducationAsync(Guid applicantId, EducationDocumentUpdateDto dto);

    Task<IReadOnlyList<DocumentScanDto>> GetScansAsync(Guid applicantId, string docType);
    Task<DocumentScanDto> UploadScanAsync(Guid applicantId, string docType, IFormFile file, string bearerToken, CancellationToken ct);

    Task<(byte[] Content, string ContentType, string FileName)> DownloadScanAsync(Guid applicantId, string docType, Guid scanId, string bearerToken, CancellationToken ct);

    Task DeleteScanAsync(Guid applicantId, string docType, Guid scanId);
}
