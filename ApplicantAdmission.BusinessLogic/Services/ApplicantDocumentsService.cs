using ApplicantAdmission.BusinessLogic.Exceptions;
using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Documents;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Entities;
using ApplicantAdmission.DataAccess.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.BusinessLogic.Services;

public class ApplicantDocumentsService : IApplicantDocumentsService
{
    private readonly ApplicantDbContext _db;
    private readonly FilesApiClient _files;

    public ApplicantDocumentsService(ApplicantDbContext db, FilesApiClient files)
    {
        _db = db;
        _files = files;
    }

    private static string NormalizeDocType(string docType)
    {
        docType = (docType ?? "").Trim().ToLowerInvariant();
        return docType switch
        {
            "passport" => "Passport",
            "education" => "Education",
            _ => throw new BusinessRuleException("Unknown document type. Use 'passport' or 'education'.")
        };
    }

    private async Task EnsureNotClosed(Guid applicantId)
    {
        var admission = await _db.ApplicantAdmissions
            .Where(x => x.ApplicantId == applicantId)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (admission != null && admission.Status == AdmissionStatus.Closed)
            throw new BusinessRuleException("Admission is Closed. Editing is not allowed.");
    }

    private async Task<PassportDocument> GetOrCreatePassport(Guid applicantId)
    {
        var doc = await _db.Documents.OfType<PassportDocument>()
            .FirstOrDefaultAsync(x => x.ApplicantId == applicantId);

        if (doc != null) return doc;

        doc = new PassportDocument
        {
            Id = Guid.NewGuid(),
            ApplicantId = applicantId,
            Series = "",
            Number = "",
            IssuedBy = "",
            IssueDate = DateTime.UtcNow.Date,
            BirthPlace = ""
        };

        _db.Documents.Add(doc);
        await _db.SaveChangesAsync();
        return doc;
    }

    
    private async Task<EducationDocument> GetOrCreateEducation(Guid applicantId)
    {
        var doc = await _db.Documents.OfType<EducationDocument>()
            .FirstOrDefaultAsync(x => x.ApplicantId == applicantId);

        if (doc != null) return doc;

        
        var defaultTypeId = await _db.EducationDocumentTypes
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        if (defaultTypeId == Guid.Empty)
            throw new BusinessRuleException("Education document types are not synced yet. Run DictionarySyncJob.");

        doc = new EducationDocument
        {
            Id = Guid.NewGuid(),
            ApplicantId = applicantId,
            Name = "",
            IssueDate = DateTime.UtcNow.Date,
            DocumentTypeId = defaultTypeId
        };

        _db.Documents.Add(doc);
        await _db.SaveChangesAsync();
        return doc;
    }

    public async Task<PassportDocumentDto> GetPassportAsync(Guid applicantId)
    {
        var doc = await _db.Documents.OfType<PassportDocument>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ApplicantId == applicantId);

        if (doc == null)
        {
            doc = await GetOrCreatePassport(applicantId);
        }

        return new PassportDocumentDto
        {
            Id = doc.Id,
            Series = doc.Series,
            Number = doc.Number,
            IssuedBy = doc.IssuedBy,
            IssueDate = doc.IssueDate,
            BirthPlace = doc.BirthPlace
        };
    }

    public async Task<PassportDocumentDto> UpsertPassportAsync(Guid applicantId, PassportDocumentUpdateDto dto)
    {
        await EnsureNotClosed(applicantId);

        var doc = await GetOrCreatePassport(applicantId);

        doc.Series = dto.Series;
        doc.Number = dto.Number;
        doc.IssuedBy = dto.IssuedBy;
        doc.IssueDate = dto.IssueDate;
        doc.BirthPlace = dto.BirthPlace;

        await _db.SaveChangesAsync();

        return await GetPassportAsync(applicantId);
    }

    public async Task<EducationDocumentDto> GetEducationAsync(Guid applicantId)
    {
        var doc = await _db.Documents.OfType<EducationDocument>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ApplicantId == applicantId);

        if (doc == null)
        {
            doc = await GetOrCreateEducation(applicantId);
        }

        
        return new EducationDocumentDto
        {
            Id = doc.Id,
            Name = doc.Name ?? "",
            DocumentTypeId = doc.DocumentTypeId,
            IssueDate = doc.IssueDate
        };
    }

    public async Task<EducationDocumentDto> UpsertEducationAsync(Guid applicantId, EducationDocumentUpdateDto dto)
    {
        await EnsureNotClosed(applicantId);

        var typeExists = await _db.EducationDocumentTypes.AnyAsync(x => x.Id == dto.DocumentTypeId);
        if (!typeExists) throw new BusinessRuleException("Invalid education document type.");

        var doc = await GetOrCreateEducation(applicantId);

        doc.Name = dto.Name;
        doc.DocumentTypeId = dto.DocumentTypeId;
        doc.IssueDate = dto.IssueDate;

        await _db.SaveChangesAsync();

        return await GetEducationAsync(applicantId);
    }

    public async Task<IReadOnlyList<DocumentScanDto>> GetScansAsync(Guid applicantId, string docType)
    {
        var kind = NormalizeDocType(docType);

        var doc = await _db.Documents
            .FirstOrDefaultAsync(x => x.ApplicantId == applicantId && x.DocumentKind == kind);

        if (doc == null)
        {
            doc = kind == "Passport"
                ? await GetOrCreatePassport(applicantId)
                : await GetOrCreateEducation(applicantId);
        }

        var scans = await _db.DocumentScans
            .Where(x => x.DocumentId == doc.Id)
            .OrderBy(x => x.CreatedAt)
            .AsNoTracking()
            .ToListAsync();

        return scans.Select(x => new DocumentScanDto
        {
            Id = x.Id,
            FileId = x.FileId,
            FileName = x.FileName,
            ContentType = x.ContentType,
            Size = x.Size,
            CreatedAt = x.CreatedAt
        }).ToList();
    }

    public async Task<DocumentScanDto> UploadScanAsync(Guid applicantId, string docType, IFormFile file, string bearerToken, CancellationToken ct)
    {
        await EnsureNotClosed(applicantId);

        if (file == null || file.Length == 0)
            throw new BusinessRuleException("File is required.");

        var kind = NormalizeDocType(docType);

        var doc = await _db.Documents
            .FirstOrDefaultAsync(x => x.ApplicantId == applicantId && x.DocumentKind == kind, ct);

        if (doc == null)
        {
            doc = kind == "Passport"
                ? await GetOrCreatePassport(applicantId)
                : await GetOrCreateEducation(applicantId);
        }

        var (fileId, storedName) = await _files.UploadAsync(file, bearerToken, ct);

        var scan = new DocumentScan
        {
            Id = Guid.NewGuid(),
            DocumentId = doc.Id,
            FileId = fileId,
            FileName = storedName,
            ContentType = file.ContentType ?? "application/octet-stream",
            Size = file.Length,
            CreatedAt = DateTime.UtcNow
        };

        _db.DocumentScans.Add(scan);
        await _db.SaveChangesAsync(ct);

        return new DocumentScanDto
        {
            Id = scan.Id,
            FileId = scan.FileId,
            FileName = scan.FileName,
            ContentType = scan.ContentType,
            Size = scan.Size,
            CreatedAt = scan.CreatedAt
        };
    }

    public async Task<(byte[] Content, string ContentType, string FileName)> DownloadScanAsync(Guid applicantId, string docType, Guid scanId, string bearerToken, CancellationToken ct)
    {
        var kind = NormalizeDocType(docType);

        var scan = await _db.DocumentScans
            .Include(x => x.Document)
            .FirstOrDefaultAsync(x => x.Id == scanId, ct);

        if (scan == null) throw new NotFoundException("Scan not found.");

        if (scan.Document.ApplicantId != applicantId || scan.Document.DocumentKind != kind)
            throw new NotFoundException("Scan not found.");

        var file = await _files.DownloadAsync(scan.FileId, bearerToken, ct);
        if (file == null) throw new NotFoundException("File not found.");

        return file.Value;
    }

    public async Task DeleteScanAsync(Guid applicantId, string docType, Guid scanId)
    {
        await EnsureNotClosed(applicantId);

        var kind = NormalizeDocType(docType);

        var scan = await _db.DocumentScans
            .Include(x => x.Document)
            .FirstOrDefaultAsync(x => x.Id == scanId);

        if (scan == null) throw new NotFoundException("Scan not found.");

        if (scan.Document.ApplicantId != applicantId || scan.Document.DocumentKind != kind)
            throw new NotFoundException("Scan not found.");

        _db.DocumentScans.Remove(scan);
        await _db.SaveChangesAsync();
    }
}
