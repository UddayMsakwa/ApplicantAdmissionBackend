
using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.SelectedPrograms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("admission/selected-programs")]
public class AdmissionSelectedProgramsController : ControllerBase
{
    private readonly ISelectedProgramsService _svc;

    public AdmissionSelectedProgramsController(ISelectedProgramsService svc)
    {
        _svc = svc;
    }

    private Guid GetUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("userId");
        if (string.IsNullOrWhiteSpace(raw) || !Guid.TryParse(raw, out var id))
            throw new UnauthorizedAccessException("Invalid or missing userId claim.");
        return id;
    }

    [HttpGet]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> GetMy()
        => Ok(await _svc.GetMyAsync(GetUserId()));

    [HttpPost]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> Add([FromBody] SelectedProgramAddDto dto)
        => Ok(await _svc.AddMyAsync(GetUserId(), dto));

    [HttpPut("{id:guid}/priority")]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> UpdatePriority(Guid id, [FromBody] SelectedProgramPriorityDto dto)
        => Ok(await _svc.UpdatePriorityMyAsync(GetUserId(), id, dto));

    [HttpDelete("{programId:guid}")]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> Remove(Guid programId)
        => Ok(await _svc.RemoveMyAsync(GetUserId(), programId));
}
