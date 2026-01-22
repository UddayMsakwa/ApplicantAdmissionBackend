
using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Authorize(Policy = "ManagerAccess")]
public class ManagerAdmissionsController : ControllerBase
{
    private readonly IAdmissionService _admissions;

    public ManagerAdmissionsController(IAdmissionService admissions)
    {
        _admissions = admissions;
    }

    private Guid GetUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("userId");
        if (string.IsNullOrWhiteSpace(raw) || !Guid.TryParse(raw, out var id))
            throw new UnauthorizedAccessException("Invalid or missing userId claim.");
        return id;
    }

    private async Task<bool> IsAssignedManager(Guid admissionId, Guid actorUserId)
    {
        var entity = await _admissions.GetEntityForOwnershipCheckAsync(admissionId);
        return entity.ManagerUserId.HasValue && entity.ManagerUserId.Value == actorUserId;
    }

    
    [HttpPatch("manager/admissions/{admissionId:guid}/status")]
    public async Task<IActionResult> PatchStatus([FromRoute] Guid admissionId, [FromBody] ApplicantAdmissionUpdateStatusDto dto)
    {
        var actorUserId = GetUserId();

        if (User.IsInRole("Manager") && !User.IsInRole("HeadManager"))
        {
            if (!await IsAssignedManager(admissionId, actorUserId))
                return Forbid();
        }

        return Ok(await _admissions.UpdateStatusAsync(admissionId, dto.Status));
    }

    
    [HttpPatch("manager/applications/{admissionId:guid}/status")]
    public Task<IActionResult> PatchStatusApplications([FromRoute] Guid admissionId, [FromBody] ApplicantAdmissionUpdateStatusDto dto)
        => PatchStatus(admissionId, dto);


}
