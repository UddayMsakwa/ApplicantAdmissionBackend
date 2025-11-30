using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;
using Microsoft.AspNetCore.Mvc;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("api/applicants")]
public class ApplicantController : ControllerBase
{
    private readonly IApplicantService _service;

    public ApplicantController(IApplicantService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var applicant = await _service.GetByIdAsync(id);
        return applicant == null ? NotFound() : Ok(applicant);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ApplicantCreateDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return Ok(created);
    }
}
