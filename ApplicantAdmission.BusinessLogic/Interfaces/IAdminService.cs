using ApplicantAdmission.BusinessLogic.Models.Dtos.Admin;

namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface IAdminService
{
    Task<List<UserDto>> GetStaffAsync();
    Task<UserDto> CreateStaffAsync(CreateStaffDto dto);
    Task<UserDto> UpdateStaffAsync(Guid id, UpdateStaffDto dto);
    Task DeleteStaffAsync(Guid id);
}
