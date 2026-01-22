using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;
using ApplicantAdmission.DataAccess.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("admissions")]
public class AdmissionController : ControllerBase
{
    private readonly IAdmissionService _service;

    public AdmissionController(IAdmissionService service)
    {
        _service = service;
    }

    private Guid GetUserId()
    {
        
        var raw =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("userId");

        if (string.IsNullOrWhiteSpace(raw) || !Guid.TryParse(raw, out var id))
            throw new UnauthorizedAccessException("Invalid or missing userId claim.");

        return id;
    }

    [HttpGet("{id:guid}")]
    [Authorize] 
    public async Task<IActionResult> GetById(Guid id)
    {
        var admission = await _service.GetByIdAsync(id);
        return admission == null ? NotFound() : Ok(admission);
    }

    
    [HttpGet]
    [Authorize(Roles = "Admin,HeadManager,Manager")]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(await _service.GetPagedAsync(page, pageSize));
    }

    
    [HttpGet("me")]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> GetMy()
    {
        var userId = GetUserId();
        return Ok(await _service.GetMyAsync(userId));
    }

    
    [HttpPost]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> Create([FromBody] ApplicantAdmissionCreateDto dto)
    {
        var userId = GetUserId();
        var created = await _service.CreateMyAsync(userId, dto);
        return Ok(created);
    }

    
    [HttpGet("assigned")]
    [Authorize(Roles = "Manager,HeadManager")]
    public async Task<IActionResult> GetAssignedAdmissions()
    {
        var userId = GetUserId();
        return Ok(await _service.GetByManagerAsync(userId));
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Manager,HeadManager")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] ApplicantAdmissionUpdateStatusDto dto)
    {
        var actorUserId = GetUserId();

        
        if (User.IsInRole("Manager") && !User.IsInRole("HeadManager"))
        {
            var admission = await _service.GetEntityForOwnershipCheckAsync(id); 
            if (admission.ManagerUserId == null || admission.ManagerUserId.Value != actorUserId)
                return Forbid();
        }

        return Ok(await _service.UpdateStatusAsync(id, dto.Status));
    }



    [HttpPost("{id:guid}/assign-manager")]
    [Authorize(Roles = "HeadManager")]
    public async Task<IActionResult> AssignManager(Guid id, [FromBody] ApplicantAdmissionAssignManagerDto dto)
    {
        await _service.AssignManagerAsync(id, dto.ManagerId);
        return NoContent();
    }

    
    [HttpGet("by-applicant/{applicantId:guid}")]
    [Authorize(Roles = "Admin,HeadManager,Manager")]
    public async Task<IActionResult> GetByApplicant(Guid applicantId)
    {
        return Ok(await _service.GetByApplicantAsync(applicantId));
    }

    
    [HttpGet("by-manager/{managerId:guid}")]
    [Authorize(Roles = "Admin,HeadManager")]
    public async Task<IActionResult> GetByManager(Guid managerId)
    {
        return Ok(await _service.GetByManagerAsync(managerId));
    }
}
