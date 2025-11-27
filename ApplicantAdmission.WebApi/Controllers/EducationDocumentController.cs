using Microsoft.AspNetCore.Mvc;
using ApplicantAdmission.BusinessLogic.Interfaces;

[ApiController]
[Route("api/education-documents")]
public class EducationDocumentController : ControllerBase
{
    private readonly IEducationDocumentService _service;

    public EducationDocumentController(IEducationDocumentService service)
    {
        _service = service;
    }

    [HttpGet("by-applicant/{applicantId:guid}")]
    public async Task<IActionResult> GetByApplicant(Guid applicantId)
    {
        return Ok(await _service.GetByApplicantAsync(applicantId));
    }
}
