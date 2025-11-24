using ApplicantAdmission.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("api/programs")]
public class ProgramController : ControllerBase
{
    private readonly IProgramService _service;

    public ProgramController(IProgramService service)
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
        var program = await _service.GetByIdAsync(id);
        return program == null ? NotFound() : Ok(program);
    }
}
