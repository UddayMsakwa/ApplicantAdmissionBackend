using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("profile")]
[Authorize(Policy = "ApplicantOnly")]
public class ProfileController : ControllerBase
{
    private readonly IApplicantService _applicants;
    private readonly IAuthService _auth;

    public ProfileController(IApplicantService applicants, IAuthService auth)
    {
        _applicants = applicants;
        _auth = auth;
    }

    private Guid GetUserId()
    {
        var raw =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("userId") ??
            User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrWhiteSpace(raw) || !Guid.TryParse(raw, out var id))
            throw new UnauthorizedAccessException("Invalid or missing userId claim.");

        return id;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = GetUserId();
        return Ok(await _applicants.GetMeAsync(userId));
    }

   
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] ApplicantUpdateDto dto)
    {
        var userId = GetUserId();
        return Ok(await _applicants.UpdateMeAsync(userId, dto));
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userId = GetUserId();
        await _auth.ChangePasswordAsync(userId, dto);
        return Ok();
    }
}
