using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Education;
using Microsoft.AspNetCore.Mvc;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("api/education-documents")]
public class EducationDocumentController : ControllerBase
{
    private readonly IEducationDocumentService _service;

    public EducationDocumentController(IEducationDocumentService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EducationDocumentCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return Ok(result);
    }

    [HttpGet("by-applicant/{applicantId:guid}")]
    public async Task<IActionResult> GetByApplicant(Guid applicantId)
    {
        return Ok(await _service.GetByApplicantAsync(applicantId));
    }
}
