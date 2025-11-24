using ApplicantAdmission.BusinessLogic.Models.Manager;

namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface IManagerService
{
    Task<ManagerDto?> GetByIdAsync(Guid id);
    Task<List<ManagerDto>> GetAllAsync();
}
