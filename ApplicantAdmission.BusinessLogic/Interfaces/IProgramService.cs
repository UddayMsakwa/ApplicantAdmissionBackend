using ApplicantAdmission.BusinessLogic.Models.Dtos.Program;
using ApplicantAdmission.BusinessLogic.Models.Pagination;

namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface IProgramService
{
    Task<PagedResult<ProgramDto>> GetPagedAsync(
        int page,
        int pageSize,
        Guid? facultyId,
        int? levelId,
        string? studyForm,
        string? language,
        string? search);

    Task<ProgramDto?> GetByIdAsync(Guid id);
}
