using System.ComponentModel.DataAnnotations;

namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Auth;

public class AuthRegisterDto
{
    [Required, MaxLength(200)]
    public string FullName { get; set; } = default!;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = default!;

    [Required, MaxLength(30)]
    public string Phone { get; set; } = default!;

    [Required, MinLength(8), MaxLength(200)]
    public string Password { get; set; } = default!;
}
