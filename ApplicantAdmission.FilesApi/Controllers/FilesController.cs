using ApplicantAdmission.FilesApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApplicantAdmission.FilesApi.Controllers;

[ApiController]
[Route("api/files")]
[Authorize] 
public class FilesController : ControllerBase
{
    private readonly IFileStorage _storage;

    public FilesController(IFileStorage storage)
    {
        _storage = storage;
    }

    
    [HttpPost("upload")]
    [Authorize(Roles = "Applicant")]
    [RequestSizeLimit(20_000_000)]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0) return BadRequest("File is required.");

        var stored = await _storage.SaveAsync(file, ct);
        return Ok(new { fileId = stored.FileId, fileName = stored.FileName });
    }

    
    [HttpGet("{fileId:guid}")]
    [Authorize(Roles = "Manager,HeadManager,Admin")]
    public async Task<IActionResult> Download(Guid fileId, CancellationToken ct)
    {
        var result = await _storage.GetAsync(fileId, ct);
        if (result == null) return NotFound();

        return File(result.Content, result.ContentType, result.FileName);
    }
}


