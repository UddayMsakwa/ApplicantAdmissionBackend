
using ApplicantAdmission.BusinessLogic.Exceptions;
using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Enums;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.BusinessLogic.Services;

public sealed class AdmissionLockService : IAdmissionLockService
{
    private readonly ApplicantDbContext _db;

    public AdmissionLockService(ApplicantDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> GetLatestAdmissionIdOrThrowAsync(Guid applicantId)
    {
        var latest = await _db.ApplicantAdmissions
            .Where(a => a.ApplicantId == applicantId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new { a.Id })
            .FirstOrDefaultAsync();

        if (latest == null)
            throw new NotFoundException("Admission not found.");

        return latest.Id;
    }

    public async Task EnsureApplicantNotClosedAsync(Guid applicantId)
    {
        var latest = await _db.ApplicantAdmissions
            .Where(a => a.ApplicantId == applicantId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new { a.Status })
            .FirstOrDefaultAsync();

        if (latest == null) return;

        if (latest.Status == AdmissionStatus.Closed)
            throw new BusinessRuleException("Admission is Closed. Editing is запрещено.");
    }
}
