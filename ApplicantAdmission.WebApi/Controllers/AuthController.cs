using Microsoft.AspNetCore.Mvc;
using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Auth;

namespace ApplicantAdmission.WebApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRegisterDto dto)
        {
            var result = await _authService.RegisterApplicantAsync(dto);
            
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthLoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }
    }
}
