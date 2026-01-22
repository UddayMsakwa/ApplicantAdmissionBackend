using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Documents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("documents")]
[Authorize(Policy = "ApplicantOnly")]
public class DocumentsController : ControllerBase
{
    private readonly IApplicantService _applicants;
    private readonly IApplicantDocumentsService _docs;

    public DocumentsController(IApplicantService applicants, IApplicantDocumentsService docs)
    {
        _applicants = applicants;
        _docs = docs;
    }

    private Guid GetUserId()
    {
        var raw =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("userId") ??
            User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrWhiteSpace(raw) || !Guid.TryParse(raw, out var id))
            throw new UnauthorizedAccessException("Invalid or missing userId claim.");

        return id;
    }

    private async Task<Guid> GetApplicantId()
    {
        var userId = GetUserId();
        var me = await _applicants.GetMeAsync(userId);
        return me.Id;
    }

    private string GetBearerToken()
    {
        var auth = Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(auth) || !auth.StartsWith("Bearer "))
            throw new UnauthorizedAccessException("Missing bearer token.");

        return auth["Bearer ".Length..].Trim();
    }

    
    [HttpGet("passport")]
    public async Task<IActionResult> GetPassport()
    {
        var applicantId = await GetApplicantId();
        return Ok(await _docs.GetPassportAsync(applicantId));
    }

    
    [HttpPut("passport")]
    public async Task<IActionResult> PutPassport([FromBody] PassportDocumentUpdateDto dto)
    {
        var applicantId = await GetApplicantId();
        return Ok(await _docs.UpsertPassportAsync(applicantId, dto));
    }

    
    [HttpGet("education")]
    public async Task<IActionResult> GetEducation()
    {
        var applicantId = await GetApplicantId();
        return Ok(await _docs.GetEducationAsync(applicantId));
    }

    
    [HttpPut("education")]
    public async Task<IActionResult> PutEducation([FromBody] EducationDocumentUpdateDto dto)
    {
        var applicantId = await GetApplicantId();
        return Ok(await _docs.UpsertEducationAsync(applicantId, dto));
    }

    [HttpPost("{docType}/scans")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(20_000_000)]
    public async Task<IActionResult> UploadScan(
    [FromRoute] string docType,
    [FromForm(Name = "file")] IFormFile file,
    CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required.");

        var applicantId = await GetApplicantId();
        var token = GetBearerToken();

        return Ok(await _docs.UploadScanAsync(applicantId, docType, file, token, ct));
    }



    [HttpGet("{docType}/scans")]
    public async Task<IActionResult> GetScans([FromRoute] string docType)
    {
        var applicantId = await GetApplicantId();
        return Ok(await _docs.GetScansAsync(applicantId, docType));
    }

    
    [HttpGet("{docType}/scans/{scanId:guid}/download")]
    public async Task<IActionResult> DownloadScan([FromRoute] string docType, [FromRoute] Guid scanId, CancellationToken ct)
    {
        var applicantId = await GetApplicantId();
        var token = GetBearerToken();
        var file = await _docs.DownloadScanAsync(applicantId, docType, scanId, token, ct);

        return File(file.Content, file.ContentType, file.FileName);
    }

    
    [HttpDelete("{docType}/scans/{scanId:guid}")]
    public async Task<IActionResult> DeleteScan([FromRoute] string docType, [FromRoute] Guid scanId)
    {
        var applicantId = await GetApplicantId();
        await _docs.DeleteScanAsync(applicantId, docType, scanId);
        return Ok();
    }
}
