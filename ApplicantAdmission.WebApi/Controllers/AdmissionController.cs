using ApplicantAdmission.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("api/admissions")]
public class AdmissionController : ControllerBase
{
    private readonly IAdmissionService _service;

    public AdmissionController(IAdmissionService service)
    {
        _service = service;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var admission = await _service.GetByIdAsync(id);
        return admission == null ? NotFound() : Ok(admission);
    }

    [HttpGet("by-applicant/{applicantId:guid}")]
    public async Task<IActionResult> GetByApplicant(Guid applicantId)
    {
        return Ok(await _service.GetByApplicantAsync(applicantId));
    }
}
