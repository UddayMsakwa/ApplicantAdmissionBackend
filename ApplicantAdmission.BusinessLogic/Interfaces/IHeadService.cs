using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Manager;

namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface IHeadService
{
    Task<List<ManagerDto>> GetAllManagersAsync();
    Task<ApplicantAdmissionDto> AssignManagerAsync(Guid admissionId, Guid managerId);
}
