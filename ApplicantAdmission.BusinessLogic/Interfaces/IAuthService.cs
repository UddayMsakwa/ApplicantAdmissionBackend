using ApplicantAdmission.BusinessLogic.Models.Dtos.Auth;


namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterApplicantAsync(AuthRegisterDto dto);
    Task<AuthResponseDto> LoginAsync(AuthLoginDto dto);
}
