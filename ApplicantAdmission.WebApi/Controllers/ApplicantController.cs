using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("applicants")]
public class ApplicantController : ControllerBase
{
    private readonly IApplicantService _service;

    public ApplicantController(IApplicantService service)
    {
        _service = service;
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var applicant = await _service.GetByIdAsync(id);
        return applicant == null ? NotFound() : Ok(applicant);
    }

    [Authorize(Policy = "ApplicantOnly")]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _service.GetMeAsync(userId));
    }

    [Authorize(Policy = "ApplicantOnly")]
    [HttpPatch("me")]
    public async Task<IActionResult> UpdateMe([FromBody] ApplicantUpdateDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _service.UpdateMeAsync(userId, dto));
    }
}
