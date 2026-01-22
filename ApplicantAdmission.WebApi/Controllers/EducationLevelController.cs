using ApplicantAdmission.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("education-levels")]
public class EducationLevelController : ControllerBase
{
    private readonly IEducationLevelService _service;

    public EducationLevelController(IEducationLevelService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }
}
