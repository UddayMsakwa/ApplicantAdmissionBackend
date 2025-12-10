using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Document;
using Microsoft.AspNetCore.Mvc;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("api/documents")]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService _service;

    public DocumentController(IDocumentService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DocumentCreateDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return Ok(created);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var doc = await _service.GetByIdAsync(id);
        return doc == null ? NotFound() : Ok(doc);
    }

    [HttpGet("by-applicant/{applicantId:guid}")]
    public async Task<IActionResult> GetByApplicant(Guid applicantId)
    {
        return Ok(await _service.GetByApplicantAsync(applicantId));
    }
}
