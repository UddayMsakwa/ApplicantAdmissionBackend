namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Auth;

public class ChangePasswordDto
{
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
