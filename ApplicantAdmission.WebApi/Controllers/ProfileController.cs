using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize(Policy = "ApplicantOnly")]
public class ProfileController : ControllerBase
{
    private readonly IAuthService _auth;

    public ProfileController(IAuthService auth) => _auth = auth;

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userId = Guid.Parse(User.FindFirst("userId")!.Value);
        await _auth.ChangePasswordAsync(userId, dto);
        return Ok();
    }
}
