using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Program;
using ApplicantAdmission.BusinessLogic.Models.Pagination;
using ApplicantAdmission.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.BusinessLogic.Services;

public class ProgramService : IProgramService
{
    private readonly ApplicantDbContext _context;

    public ProgramService(ApplicantDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProgramDto>> GetPagedAsync(
        int page,
        int pageSize,
        Guid? facultyId,
        int? levelId,
        string? studyForm,
        string? language,
        string? search)
    {
        var q = _context.Programs
            .AsNoTracking()
            .Include(x => x.Faculty)
            .Include(x => x.Level)
            .AsQueryable();

        if (facultyId.HasValue)
            q = q.Where(x => x.FacultyId == facultyId.Value);

        if (levelId.HasValue)
            q = q.Where(x => x.LevelId == levelId.Value);

        if (!string.IsNullOrWhiteSpace(studyForm))
        {
            var sf = studyForm.Trim().ToLowerInvariant();
            q = q.Where(x => x.StudyForm.ToLower() == sf);
        }

        if (!string.IsNullOrWhiteSpace(language))
        {
            var lang = language.Trim().ToLowerInvariant();
            q = q.Where(x => x.Language.ToLower() == lang);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();
            q = q.Where(x =>
                x.Name.ToLower().Contains(s) ||
                x.Code.ToLower().Contains(s));
        }

        var total = await q.CountAsync();

        var items = await q
            .OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ProgramDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Language = x.Language,
                StudyForm = x.StudyForm,
                FacultyId = x.FacultyId,
                FacultyName = x.Faculty.Name,
                LevelId = x.LevelId,
                LevelName = x.Level.Name
            })
            .ToListAsync();

        return new PagedResult<ProgramDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            Items = items
        };
    }

    public async Task<ProgramDto?> GetByIdAsync(Guid id)
    {
        return await _context.Programs
            .AsNoTracking()
            .Include(x => x.Faculty)
            .Include(x => x.Level)
            .Where(x => x.Id == id)
            .Select(x => new ProgramDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Language = x.Language,
                StudyForm = x.StudyForm,
                FacultyId = x.FacultyId,
                FacultyName = x.Faculty.Name,
                LevelId = x.LevelId,
                LevelName = x.Level.Name
            })
            .FirstOrDefaultAsync();
    }
}
