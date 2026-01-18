using ApplicantAdmission.DataAccess.Enums;

namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Admin;

public class CreateStaffDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public UserRole Role { get; set; }   // Admin / HeadManager / Manager
    public string? FullName { get; set; } // only required for Manager
}

