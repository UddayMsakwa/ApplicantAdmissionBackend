using ApplicantAdmission.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("api/education-document-types")]
public class EducationDocumentTypeController : ControllerBase
{
    private readonly IEducationDocumentTypeService _service;

    public EducationDocumentTypeController(IEducationDocumentTypeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }
}
