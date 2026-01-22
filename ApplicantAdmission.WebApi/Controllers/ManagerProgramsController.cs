
using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.SelectedPrograms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("manager/applicants/{applicantId:guid}/selected-programs")]
[Authorize(Policy = "ManagerAccess")]
public class ManagerProgramsController : ControllerBase
{
    private readonly ISelectedProgramsService _svc;

    public ManagerProgramsController(ISelectedProgramsService svc)
    {
        _svc = svc;
    }

    private Guid GetActorUserId()
    {
        var sub = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrWhiteSpace(sub) || !Guid.TryParse(sub, out var id))
            throw new UnauthorizedAccessException("Invalid token subject (sub).");
        return id;
    }

    private bool BypassOwnership()
        => User.IsInRole("Admin") || User.IsInRole("HeadManager");

    [HttpGet]
    public async Task<IActionResult> Get(Guid applicantId)
        => Ok(await _svc.GetForApplicantAsync(applicantId));

    [HttpPut("{id:guid}/priority")]
    public async Task<IActionResult> UpdatePriority(Guid applicantId, Guid id, [FromBody] SelectedProgramPriorityDto dto)
        => Ok(await _svc.UpdatePriorityForApplicantAsync(applicantId, id, dto, GetActorUserId(), BypassOwnership()));

    [HttpDelete("{programId:guid}")]
    public async Task<IActionResult> Remove(Guid applicantId, Guid programId)
        => Ok(await _svc.RemoveForApplicantAsync(applicantId, programId, GetActorUserId(), BypassOwnership()));
}
