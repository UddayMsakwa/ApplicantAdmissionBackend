
using System.IdentityModel.Tokens.Jwt;
using ApplicantAdmission.BusinessLogic.Exceptions;
using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Documents;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.WebApi.Controllers;

[ApiController]
[Route("manager/applicants/{applicantId:guid}/documents")]
[Authorize(Policy = "ManagerAccess")]
public class ManagerDocumentsController : ControllerBase
{
    private readonly ApplicantDbContext _db;
    private readonly IApplicantDocumentsService _docs;

    public ManagerDocumentsController(ApplicantDbContext db, IApplicantDocumentsService docs)
    {
        _db = db;
        _docs = docs;
    }

    private Guid GetActorUserId()
    {
        var sub = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrWhiteSpace(sub) || !Guid.TryParse(sub, out var id))
            throw new UnauthorizedException("Missing or invalid token subject (sub).");

        return id;
    }

    private bool BypassOwnership()
        => User.IsInRole("Admin") || User.IsInRole("HeadManager");

    private string GetBearerToken()
    {
        var auth = Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(auth) || !auth.StartsWith("Bearer "))
            throw new UnauthorizedException("Missing bearer token.");

        return auth["Bearer ".Length..].Trim();
    }

    private async Task<(Guid AdmissionId, AdmissionStatus Status, Guid? ManagerUserId)> GetLatestAdmissionOrThrow(Guid applicantId)
    {
        var admission = await _db.ApplicantAdmissions
            .Where(a => a.ApplicantId == applicantId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new { a.Id, a.Status, a.ManagerUserId })
            .FirstOrDefaultAsync();

        if (admission == null)
            throw new NotFoundException("Admission not found for applicant.");

        return (admission.Id, admission.Status, admission.ManagerUserId);
    }

    private async Task EnsureCanEditApplicant(Guid applicantId)
    {
        if (BypassOwnership()) return;

        var actorUserId = GetActorUserId();
        var admission = await GetLatestAdmissionOrThrow(applicantId);

        
        if (admission.Status == AdmissionStatus.Closed)
            throw new ConflictException("Admission is Closed. Editing is not allowed.");

        
        if (admission.ManagerUserId == null || admission.ManagerUserId.Value != actorUserId)
            throw new ConflictException("Manager can edit only applicants assigned to them.");
    }

    private async Task EnsureNotClosed(Guid applicantId)
    {
        var admission = await GetLatestAdmissionOrThrow(applicantId);
        if (admission.Status == AdmissionStatus.Closed)
            throw new ConflictException("Admission is Closed. Editing is not allowed.");
    }

    

    
    [HttpGet("passport")]
    public async Task<IActionResult> GetPassport([FromRoute] Guid applicantId)
        => Ok(await _docs.GetPassportAsync(applicantId));

    
    [HttpPut("passport")]
    public async Task<IActionResult> PutPassport([FromRoute] Guid applicantId, [FromBody] PassportDocumentUpdateDto dto)
    {
        await EnsureCanEditApplicant(applicantId);
        return Ok(await _docs.UpsertPassportAsync(applicantId, dto));
    }


    

    [HttpGet("education")]
    public async Task<IActionResult> GetEducation([FromRoute] Guid applicantId)
        => Ok(await _docs.GetEducationAsync(applicantId));

    [HttpPut("education")]
    public async Task<IActionResult> PutEducation([FromRoute] Guid applicantId, [FromBody] EducationDocumentUpdateDto dto)
    {
        await EnsureCanEditApplicant(applicantId);
        return Ok(await _docs.UpsertEducationAsync(applicantId, dto));
    }

    
    [HttpPost("{docType}/scans")]
    [RequestSizeLimit(20_000_000)]
    public async Task<IActionResult> UploadScan(
        [FromRoute] Guid applicantId,
        [FromRoute] string docType,
        IFormFile file,
        CancellationToken ct)
    {
        await EnsureCanEditApplicant(applicantId);
        var token = GetBearerToken();
        return Ok(await _docs.UploadScanAsync(applicantId, docType, file, token, ct));
    }

    
    [HttpGet("{docType}/scans")]
    public async Task<IActionResult> GetScans([FromRoute] Guid applicantId, [FromRoute] string docType)
        => Ok(await _docs.GetScansAsync(applicantId, docType));

   
    [HttpGet("{docType}/scans/{scanId:guid}/download")]
    public async Task<IActionResult> DownloadScan(
        [FromRoute] Guid applicantId,
        [FromRoute] string docType,
        [FromRoute] Guid scanId,
        CancellationToken ct)
    {
        var token = GetBearerToken();
        var file = await _docs.DownloadScanAsync(applicantId, docType, scanId, token, ct);
        return File(file.Content, file.ContentType, file.FileName);
    }

    
    [HttpDelete("{docType}/scans/{scanId:guid}")]
    public async Task<IActionResult> DeleteScan([FromRoute] Guid applicantId, [FromRoute] string docType, [FromRoute] Guid scanId)
    {
        await EnsureCanEditApplicant(applicantId);
        await _docs.DeleteScanAsync(applicantId, docType, scanId);
        return Ok();
    }
}
