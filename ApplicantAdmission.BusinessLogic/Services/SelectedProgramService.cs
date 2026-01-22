using ApplicantAdmission.BusinessLogic.Exceptions;
using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.SelectedPrograms;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Entities;
using ApplicantAdmission.DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace ApplicantAdmission.BusinessLogic.Services;

public sealed class SelectedProgramsService : ISelectedProgramsService
{
    private readonly ApplicantDbContext _db;
    private readonly IAdmissionLockService _lock;
    private readonly IConfiguration _cfg;

    public SelectedProgramsService(ApplicantDbContext db, IAdmissionLockService @lock, IConfiguration cfg)
    {
        _db = db;
        _lock = @lock;
        _cfg = cfg;
    }

    private int MaxSelectedPrograms => _cfg.GetValue<int>("Admission:MaxSelectedPrograms", 3);

    private async Task<Guid> GetApplicantIdByUserIdOrThrow(Guid userId)
    {
        var applicantId = await _db.Applicants
            .Where(a => a.UserId == userId)
            .Select(a => a.Id)
            .FirstOrDefaultAsync();

        if (applicantId == Guid.Empty)
            throw new NotFoundException("Applicant profile not found.");

        return applicantId;
    }

    

    private static int? TryGetIntId(object? obj, params string[] propNames)
    {
        if (obj == null) return null;

        var t = obj.GetType();
        foreach (var name in propNames)
        {
            var p = t.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (p == null) continue;

            var val = p.GetValue(obj);
            if (val == null) continue;

            try
            {
                if (val is int i) return i;

                
                if (Nullable.GetUnderlyingType(p.PropertyType) == typeof(int))
                    return (int?)val;

                return Convert.ToInt32(val);
            }
            catch { }
        }
        return null;
    }

    private static Guid? TryGetGuidId(object? obj, params string[] propNames)
    {
        if (obj == null) return null;

        var t = obj.GetType();
        foreach (var name in propNames)
        {
            var p = t.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (p == null) continue;

            var val = p.GetValue(obj);
            if (val == null) continue;

            if (val is Guid g) return g;

            
            if (Nullable.GetUnderlyingType(p.PropertyType) == typeof(Guid))
                return (Guid?)val;

            if (val is string s && Guid.TryParse(s, out var parsed))
                return parsed;
        }

        return null;
    }

    private static IEnumerable<int> TryGetNextLevelIds(object? eduDocType)
    {
        if (eduDocType == null) yield break;

        var t = eduDocType.GetType();

        var navNames = new[]
        {
            "NextEducationLevels",
            "NextLevels",
            "AvailableNextEducationLevels",
            "FurtherEducationLevels"
        };

        foreach (var nav in navNames)
        {
            var p = t.GetProperty(nav, BindingFlags.Public | BindingFlags.Instance);
            if (p == null) continue;

            var val = p.GetValue(eduDocType);
            if (val == null) continue;

            if (val is IEnumerable<int> ints)
            {
                foreach (var x in ints) yield return x;
                yield break;
            }

            if (val is System.Collections.IEnumerable en)
            {
                foreach (var item in en)
                {
                    var id = TryGetIntId(item, "Id", "EducationLevelId", "LevelId");
                    if (id.HasValue) yield return id.Value;
                }
                yield break;
            }
        }
    }

   

    private static List<SelectedProgramDto> Map(List<AdmissionProgram> aps)
        => aps
            .OrderBy(x => x.Priority)
            .Select(x => new SelectedProgramDto
            {
                Id = x.Id,
                ProgramId = x.ProgramId,
                ProgramName = x.Program.Name,
                FacultyId = x.Program.FacultyId,
                FacultyName = x.Program.Faculty.Name,
                EducationLevelId = x.Program.LevelId,
                EducationLevelName = x.Program.Level.Name,
                Priority = x.Priority
            })
            .ToList();

    private async Task<List<AdmissionProgram>> LoadPrograms(Guid admissionId)
        => await _db.AdmissionPrograms
            .Include(x => x.Program).ThenInclude(p => p.Faculty)
            .Include(x => x.Program).ThenInclude(p => p.Level)
            .Where(x => x.ApplicantAdmissionId == admissionId)
            .OrderBy(x => x.Priority)
            .ToListAsync();

    private async Task<ApplicantAdmissionEntity> LoadAdmissionForApplicant(Guid applicantId)
    {
        var admission = await _db.ApplicantAdmissions
            .Where(a => a.ApplicantId == applicantId)
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync();

        if (admission == null)
            throw new NotFoundException("Admission not found.");

        return admission;
    }

    
    private async Task EnsureLevelAllowedByEducationDoc(Guid applicantId, int programLevelId)
    {
        var eduDoc = await _db.EducationDocuments
            .FirstOrDefaultAsync(d => d.ApplicantId == applicantId);

        if (eduDoc == null) return;

        
        var eduDocTypeId = TryGetGuidId(eduDoc, "EducationDocumentTypeId", "DocumentTypeId", "TypeId");
        if (!eduDocTypeId.HasValue) return;

        var docType = await _db.EducationDocumentTypes
            .FirstOrDefaultAsync(t => t.Id == eduDocTypeId.Value);

        if (docType == null) return;

        
        var docLevelId = TryGetIntId(docType, "EducationLevelId", "LevelId");

        if (!docLevelId.HasValue)
        {
            var levelObj = docType.GetType().GetProperty("Level")?.GetValue(docType);
            docLevelId = TryGetIntId(levelObj, "Id", "EducationLevelId", "LevelId");
        }

        if (!docLevelId.HasValue) return;

        if (docLevelId.Value == programLevelId) return;

        var nextIds = TryGetNextLevelIds(docType).ToHashSet();
        if (nextIds.Contains(programLevelId)) return;

        throw new BusinessRuleException(
            "Selected program education level is not allowed by applicant education document.");
    }

    private static void RenumberPriorities(List<AdmissionProgram> aps)
    {
        aps = aps.OrderBy(x => x.Priority).ToList();
        for (var i = 0; i < aps.Count; i++)
            aps[i].Priority = i + 1;
    }

    public async Task<List<SelectedProgramDto>> GetMyAsync(Guid userId)
    {
        var applicantId = await GetApplicantIdByUserIdOrThrow(userId);
        var admission = await LoadAdmissionForApplicant(applicantId);
        var aps = await LoadPrograms(admission.Id);
        return Map(aps);
    }

    public async Task<List<SelectedProgramDto>> AddMyAsync(Guid userId, SelectedProgramAddDto dto)
    {
        var applicantId = await GetApplicantIdByUserIdOrThrow(userId);
        await _lock.EnsureApplicantNotClosedAsync(applicantId);

        var admission = await LoadAdmissionForApplicant(applicantId);

        var program = await _db.Programs
            .Include(p => p.Level)
            .FirstOrDefaultAsync(p => p.Id == dto.ProgramId);

        if (program == null)
            throw new NotFoundException("Program not found.");

        var current = await LoadPrograms(admission.Id);

        if (current.Count >= MaxSelectedPrograms)
            throw new BusinessRuleException($"Cannot select more than {MaxSelectedPrograms} programs.");

        if (current.Any(x => x.ProgramId == dto.ProgramId))
            throw new BusinessRuleException("Program already selected.");

        
        if (current.Count > 0)
        {
            var levelId = current[0].Program.LevelId;
            if (program.LevelId != levelId)
                throw new BusinessRuleException("Selected programs must belong to the same education level.");
        }

        await EnsureLevelAllowedByEducationDoc(applicantId, program.LevelId);

        var nextPriority = current.Count == 0 ? 1 : current.Max(x => x.Priority) + 1;

        _db.AdmissionPrograms.Add(new AdmissionProgram
        {
            Id = Guid.NewGuid(),
            ApplicantAdmissionId = admission.Id,
            ProgramId = program.Id,
            Priority = nextPriority
        });

        admission.LastModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var updated = await LoadPrograms(admission.Id);
        return Map(updated);
    }

    public async Task<List<SelectedProgramDto>> UpdatePriorityMyAsync(
    Guid userId,
    Guid admissionProgramId,
    SelectedProgramPriorityDto dto)
    {
        var applicantId = await GetApplicantIdByUserIdOrThrow(userId);
        await _lock.EnsureApplicantNotClosedAsync(applicantId);

        var admission = await LoadAdmissionForApplicant(applicantId);
        var aps = await LoadPrograms(admission.Id);

        var target = aps.FirstOrDefault(x => x.Id == admissionProgramId);
        if (target == null) throw new NotFoundException("Selected program not found.");

        if (dto.Priority < 1 || dto.Priority > aps.Count)
            throw new BusinessRuleException("Invalid priority.");

        
        var reordered = aps
            .OrderBy(x => x.Priority)
            .ToList();

        reordered.Remove(target);
        reordered.Insert(dto.Priority - 1, target);

        
        for (var i = 0; i < reordered.Count; i++)
            reordered[i].Priority = 1000 + i + 1;

        admission.LastModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        
        for (var i = 0; i < reordered.Count; i++)
            reordered[i].Priority = i + 1;

        admission.LastModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var updated = await LoadPrograms(admission.Id);
        return Map(updated);
    }


    public async Task<List<SelectedProgramDto>> RemoveMyAsync(Guid userId, Guid programId)
    {
        var applicantId = await GetApplicantIdByUserIdOrThrow(userId);
        await _lock.EnsureApplicantNotClosedAsync(applicantId);

        var admission = await LoadAdmissionForApplicant(applicantId);
        var aps = await LoadPrograms(admission.Id);

        var target = aps.FirstOrDefault(x => x.ProgramId == programId);
        if (target == null) throw new NotFoundException("Selected program not found.");

        _db.AdmissionPrograms.Remove(target);

        aps.Remove(target);
        RenumberPriorities(aps);

        admission.LastModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var updated = await LoadPrograms(admission.Id);
        return Map(updated);
    }

    public async Task<List<SelectedProgramDto>> GetForApplicantAsync(Guid applicantId)
    {
        var admission = await LoadAdmissionForApplicant(applicantId);
        var aps = await LoadPrograms(admission.Id);
        return Map(aps);
    }

    private async Task EnsureOwnershipOrThrow(Guid applicantId, Guid actorUserId, bool bypassOwnership)
    {
        if (bypassOwnership) return;

        var admission = await LoadAdmissionForApplicant(applicantId);

        if (admission.ManagerUserId == null || admission.ManagerUserId.Value != actorUserId)
            throw new UnauthorizedException("Manager can edit only assigned applicants.");
    }

    public async Task<List<SelectedProgramDto>> UpdatePriorityForApplicantAsync(
    Guid applicantId,
    Guid admissionProgramId,
    SelectedProgramPriorityDto dto,
    Guid actorUserId,
    bool bypassOwnership)
    {
        await EnsureOwnershipOrThrow(applicantId, actorUserId, bypassOwnership);

        var admission = await LoadAdmissionForApplicant(applicantId);
        if (admission.Status == AdmissionStatus.Closed)
            throw new BusinessRuleException("Admission is Closed and cannot be changed.");

        var aps = await LoadPrograms(admission.Id);

        var target = aps.FirstOrDefault(x => x.Id == admissionProgramId);
        if (target == null) throw new NotFoundException("Selected program not found.");

        if (dto.Priority < 1 || dto.Priority > aps.Count)
            throw new BusinessRuleException("Invalid priority.");

       
        var reordered = aps
            .OrderBy(x => x.Priority)
            .ToList();

        reordered.Remove(target);
        reordered.Insert(dto.Priority - 1, target);

        
        for (var i = 0; i < reordered.Count; i++)
            reordered[i].Priority = 1000 + i + 1;

        admission.LastModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        for (var i = 0; i < reordered.Count; i++)
            reordered[i].Priority = i + 1;

        admission.LastModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var updated = await LoadPrograms(admission.Id);
        return Map(updated);
    }


    public async Task<List<SelectedProgramDto>> RemoveForApplicantAsync(Guid applicantId, Guid programId, Guid actorUserId, bool bypassOwnership)
    {
        await EnsureOwnershipOrThrow(applicantId, actorUserId, bypassOwnership);

        var admission = await LoadAdmissionForApplicant(applicantId);
        if (admission.Status == AdmissionStatus.Closed)
            throw new BusinessRuleException("Admission is Closed and cannot be changed.");

        var aps = await LoadPrograms(admission.Id);
        var target = aps.FirstOrDefault(x => x.ProgramId == programId);
        if (target == null) throw new NotFoundException("Selected program not found.");

        _db.AdmissionPrograms.Remove(target);

        aps.Remove(target);
        RenumberPriorities(aps);

        admission.LastModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var updated = await LoadPrograms(admission.Id);
        return Map(updated);
    }
}
