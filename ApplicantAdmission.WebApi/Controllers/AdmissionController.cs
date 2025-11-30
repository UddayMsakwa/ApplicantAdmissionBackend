using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Get(Guid id)
        {
            var admission = await _service.GetByIdAsync(id);
            return admission == null ? NotFound() : Ok(admission);
        }

        [HttpGet("by-applicant/{applicantId:guid}")]
        public async Task<IActionResult> GetByApplicant(Guid applicantId)
        {
            return Ok(await _service.GetByApplicantAsync(applicantId));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApplicantAdmissionCreateDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [HttpPost("{id:guid}/assign-manager")]
        public async Task<IActionResult> AssignManager(Guid id, [FromBody] ApplicantAdmissionAssignManagerDto dto)
        {
            var updated = await _service.AssignManagerAsync(id, dto.ManagerId);
            return Ok(updated);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] ApplicantAdmissionUpdateStatusDto dto)
        {
            var updated = await _service.UpdateStatusAsync(id, dto.Status);
            return Ok(updated);
        }
    }
}
