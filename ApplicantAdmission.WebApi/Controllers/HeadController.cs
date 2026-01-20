using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Head;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("api/head")]
[Authorize(Policy = "HeadAccess")]
public class HeadController : ControllerBase
{
    private readonly IHeadService _service;

    public HeadController(IHeadService service) => _service = service;

    [HttpGet("managers")]
    public async Task<IActionResult> GetManagers()
        => Ok(await _service.GetAllManagersAsync());

    [HttpPost("admissions/{id:guid}/assign")]
    public async Task<IActionResult> AssignManager(Guid id, [FromBody] HeadAssignAdmissionDto dto)
        => Ok(await _service.AssignManagerAsync(id, dto.ManagerId));
}
