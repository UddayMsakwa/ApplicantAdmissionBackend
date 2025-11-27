using Microsoft.AspNetCore.Mvc;
using ApplicantAdmission.BusinessLogic.Interfaces;

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
}
