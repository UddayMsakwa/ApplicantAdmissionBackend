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

    private Guid GetUserId()
    {
        
        var sub = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        if (sub == null) throw new UnauthorizedAccessException("Missing token subject.");
        return Guid.Parse(sub);
    }

    [HttpGet("applications")]
    public async Task<IActionResult> GetApplications(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        return Ok(await _service.GetApplicationsAsync(status, page, pageSize));
    }

    [HttpGet("applicants/{id:guid}")]
    public async Task<IActionResult> GetApplicant(Guid id)
    {
        return Ok(await _service.GetApplicantAsync(id));
    }

    [HttpPost("admissions/{id:guid}/take")]
    public async Task<IActionResult> Take(Guid id)
    {
        var managerUserId = GetUserId();
        return Ok(await _service.TakeAdmissionAsync(id, managerUserId));
    }

    [HttpPost("admissions/{id:guid}/release")]
    public async Task<IActionResult> Release(Guid id)
    {
        var managerUserId = GetUserId();
        return Ok(await _service.ReleaseAdmissionAsync(id, managerUserId));
    }

    [HttpPatch("applicants/{id:guid}")]
    public async Task<IActionResult> UpdateApplicant(Guid id, [FromBody] ApplicantUpdateDto dto)
    {
        return Ok(await _service.UpdateApplicantAsync(id, dto));
    }
}
