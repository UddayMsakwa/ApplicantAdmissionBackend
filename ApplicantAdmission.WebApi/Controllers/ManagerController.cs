using System.IdentityModel.Tokens.Jwt;
using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("api/manager")]
[Authorize(Policy = "ManagerOnly")]
public class ManagerController : ControllerBase
{
    private readonly IManagerService _service;

    public ManagerController(IManagerService service)
    {
        _service = service;
    }

    private IActionResult? TryGetUserId(out Guid userId)
    {
        userId = default;

        var sub = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrWhiteSpace(sub)) return Unauthorized("Missing token subject (sub).");

        if (!Guid.TryParse(sub, out userId)) return Unauthorized("Invalid token subject (sub).");

        return null;
    }

    [HttpGet("applications")]
    public async Task<IActionResult> GetApplications([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        => Ok(await _service.GetApplicationsAsync(status, page, pageSize));

    [HttpGet("applicants/{id:guid}")]
    public async Task<IActionResult> GetApplicant(Guid id)
        => Ok(await _service.GetApplicantAsync(id));

    [HttpPost("admissions/{id:guid}/take")]
    public async Task<IActionResult> Take(Guid id)
    {
        var bad = TryGetUserId(out var managerUserId);
        if (bad != null) return bad;

        return Ok(await _service.TakeAdmissionAsync(id, managerUserId));
    }

    [HttpPost("admissions/{id:guid}/release")]
    public async Task<IActionResult> Release(Guid id)
    {
        var bad = TryGetUserId(out var managerUserId);
        if (bad != null) return bad;

        return Ok(await _service.ReleaseAdmissionAsync(id, managerUserId));
    }

    [HttpPatch("applicants/{id:guid}")]
    public async Task<IActionResult> UpdateApplicant(Guid id, [FromBody] ApplicantUpdateDto dto)
        => Ok(await _service.UpdateApplicantAsync(id, dto));
}
