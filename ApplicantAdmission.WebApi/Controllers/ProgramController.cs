using ApplicantAdmission.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("programs")]
public class ProgramController : ControllerBase
{
    private readonly IProgramService _service;

    public ProgramController(IProgramService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? facultyId = null,
        [FromQuery] int? levelId = null,
        [FromQuery] string? studyForm = null,
        [FromQuery] string? language = null,
        [FromQuery] string? search = null)
    {
        page = page < 1 ? 1 : page;

        pageSize = pageSize < 1 ? 20 : pageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var result = await _service.GetPagedAsync(
            page,
            pageSize,
            facultyId,
            levelId,
            studyForm,
            language,
            search);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var program = await _service.GetByIdAsync(id);
        return program == null ? NotFound() : Ok(program);
    }
}
