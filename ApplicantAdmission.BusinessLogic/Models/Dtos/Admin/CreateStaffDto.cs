using ApplicantAdmission.DataAccess.Enums;

namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Admin;

public class CreateStaffDto
{
    public string Email { get; set; } = null!;
    public string TempPassword { get; set; } = null!;
    public UserRole Role { get; set; }   
    public string FullName { get; set; } = null!;
}
