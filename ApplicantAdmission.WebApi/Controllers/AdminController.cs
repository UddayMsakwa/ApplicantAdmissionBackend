using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = "AdminOnly")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _service;
    public AdminController(IAdminService service) => _service = service;

    [HttpGet("staff")]
    public async Task<IActionResult> GetStaff()
        => Ok(await _service.GetStaffAsync());

    [HttpPost("staff")]
    public async Task<IActionResult> CreateStaff([FromBody] CreateStaffDto dto)
        => Ok(await _service.CreateStaffAsync(dto));

    [HttpPut("staff/{id:guid}")]
    public async Task<IActionResult> UpdateStaff(Guid id, [FromBody] UpdateStaffDto dto)
        => Ok(await _service.UpdateStaffAsync(id, dto));

    [HttpDelete("staff/{id:guid}")]
    public async Task<IActionResult> DeleteStaff(Guid id)
    {
        await _service.DeleteStaffAsync(id);
        return NoContent();
    }
}
