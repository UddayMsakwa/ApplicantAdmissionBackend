using ApplicantAdmission.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("faculties")]
public class FacultyController : ControllerBase
{
    private readonly IFacultyService _service;

    public FacultyController(IFacultyService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }
}
