
using ApplicantAdmission.BusinessLogic.Models.Dtos.SelectedPrograms;

namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface ISelectedProgramsService
{
    Task<List<SelectedProgramDto>> GetMyAsync(Guid userId);
    Task<List<SelectedProgramDto>> AddMyAsync(Guid userId, SelectedProgramAddDto dto);
    Task<List<SelectedProgramDto>> UpdatePriorityMyAsync(Guid userId, Guid admissionProgramId, SelectedProgramPriorityDto dto);
    Task<List<SelectedProgramDto>> RemoveMyAsync(Guid userId, Guid programId);

    
    Task<List<SelectedProgramDto>> GetForApplicantAsync(Guid applicantId);
    Task<List<SelectedProgramDto>> UpdatePriorityForApplicantAsync(Guid applicantId, Guid admissionProgramId, SelectedProgramPriorityDto dto, Guid actorUserId, bool bypassOwnership);
    Task<List<SelectedProgramDto>> RemoveForApplicantAsync(Guid applicantId, Guid programId, Guid actorUserId, bool bypassOwnership);
}
