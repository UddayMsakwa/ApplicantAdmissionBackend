using ApplicantAdmission.DataAccess.Enums;

namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Admin;

public class UpdateStaffDto
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public UserRole? Role { get; set; }
    public bool? IsActive { get; set; }
    public string? FullName { get; set; } 
}
