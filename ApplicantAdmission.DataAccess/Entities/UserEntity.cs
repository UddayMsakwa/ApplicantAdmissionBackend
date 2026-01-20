using ApplicantAdmission.DataAccess.Enums;

namespace ApplicantAdmission.DataAccess.Entities;

public class UserEntity
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = "";

    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;
}
