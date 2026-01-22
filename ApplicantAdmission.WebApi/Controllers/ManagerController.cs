using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;
using ApplicantAdmission.BusinessLogic.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("manager")]
[Authorize(Policy = "ManagerAccess")]
public class ManagerController : ControllerBase
{
    private readonly IManagerService _service;

    public ManagerController(IManagerService service)
    {
        _service = service;
    }

    private Guid GetUserIdOrThrow()
    {
        var idStr =
            User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
            User.FindFirst("userId")?.Value;

        if (!Guid.TryParse(idStr, out var userId))
            throw new UnauthorizedException("Invalid token: user id missing.");

        return userId;
    }

    [HttpGet("applications")]
    public async Task<IActionResult> GetApplications(
        [FromQuery] string? search,
        [FromQuery] Guid? programId,
        [FromQuery] string? facultyIds,
        [FromQuery] string? status,
        [FromQuery] bool? unassignedOnly,
        [FromQuery] bool? assignedToMe,
        [FromQuery] string? sort,
        [FromQuery] string? order,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var managerUserId = GetUserIdOrThrow();

        return Ok(await _service.GetApplicationsAdvancedAsync(
            managerUserId,
            search,
            programId,
            facultyIds,
            status,
            unassignedOnly,
            assignedToMe,
            sort,
            order,
            page,
            pageSize));
    }

    [HttpGet("applicants/{id:guid}")]
    public async Task<IActionResult> GetApplicant(Guid id)
        => Ok(await _service.GetApplicantAsync(id));

    [HttpPost("admissions/{id:guid}/take")]
    public async Task<IActionResult> Take(Guid id)
    {
        var managerUserId = GetUserIdOrThrow();
        return Ok(await _service.TakeAdmissionAsync(id, managerUserId));
    }

    [HttpPost("admissions/{id:guid}/release")]
    public async Task<IActionResult> Release(Guid id)
    {
        var managerUserId = GetUserIdOrThrow();
        return Ok(await _service.ReleaseAdmissionAsync(id, managerUserId));
    }

    [HttpPatch("applicants/{id:guid}")]
    public async Task<IActionResult> UpdateApplicant(Guid id, [FromBody] ApplicantUpdateDto dto)
        => Ok(await _service.UpdateApplicantAsync(id, dto));
}
