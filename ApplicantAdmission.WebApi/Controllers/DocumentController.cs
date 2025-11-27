using Microsoft.AspNetCore.Mvc;
using ApplicantAdmission.BusinessLogic.Interfaces;

[ApiController]
[Route("api/documents")]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService _service;

    public DocumentController(IDocumentService service)
    {
        _service = service;
    }

    [HttpGet("by-applicant/{applicantId:guid}")]
    public async Task<IActionResult> GetByApplicant(Guid applicantId)
    {
        return Ok(await _service.GetByApplicantAsync(applicantId));
    }
}
