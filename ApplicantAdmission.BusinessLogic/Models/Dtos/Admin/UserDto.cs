using ApplicantAdmission.DataAccess.Enums;

namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Admin;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
}
