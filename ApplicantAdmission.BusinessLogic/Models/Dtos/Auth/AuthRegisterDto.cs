namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Auth;

public class AuthRegisterDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

   
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}
