using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;
using ApplicantAdmission.DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ApplicantAdmission.WebApi.Controllers
{
    [ApiController]
    [Route("api/admissions")]
    public class AdmissionController : ControllerBase
    {
        private readonly IAdmissionService _service;

        public AdmissionController(IAdmissionService service)
        {
            _service = service;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var admission = await _service.GetByIdAsync(id);
            return admission == null ? NotFound() : Ok(admission);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return Ok(await _service.GetPagedAsync(page, pageSize));
        }

        [HttpGet("by-applicant/{applicantId:guid}")]
        public async Task<IActionResult> GetByApplicant(Guid applicantId)
        {
            return Ok(await _service.GetByApplicantAsync(applicantId));
        }

        [HttpGet("by-manager/{managerId:guid}")]
        public async Task<IActionResult> GetByManager(Guid managerId)
        {
            return Ok(await _service.GetByManagerAsync(managerId));
        }

        [Authorize(Roles = "Applicant")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApplicantAdmissionCreateDto dto)
        {
            var userId = Guid.Parse(User.FindFirst("userId")!.Value);
            return Ok(await _service.CreateAsync(dto));
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("assigned")]
        public async Task<IActionResult> GetAssignedAdmissions()
        {
            var userId = Guid.Parse(User.FindFirst("userId")!.Value);
            return Ok(await _service.GetByManagerAsync(userId));
        }

        [Authorize(Roles = "Manager")]
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(
            Guid id,
            [FromBody] ApplicantAdmissionUpdateStatusDto dto)
        {
            return Ok(await _service.UpdateStatusAsync(id, dto.Status));
        }

        [Authorize(Roles = "HeadManager")]
        [HttpPost("{id:guid}/assign-manager")]
        public async Task<IActionResult> AssignManager(
            Guid id,
            [FromBody] ApplicantAdmissionAssignManagerDto dto)
        {
            await _service.AssignManagerAsync(id, dto.ManagerId);
            return NoContent();
        }
    }
}