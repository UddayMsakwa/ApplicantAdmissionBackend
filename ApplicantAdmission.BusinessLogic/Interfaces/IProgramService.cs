using ApplicantAdmission.BusinessLogic.Models.Dtos.Program;

namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface IProgramService
{
    Task<List<ProgramDto>> GetAllAsync();
    Task<ProgramDto?> GetByIdAsync(Guid id);
}
