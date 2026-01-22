using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApplicantAdmission.FilesApi.Services;

namespace ApplicantAdmission.FilesApi.Controllers;

[ApiController]
[Route("files")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IFileStorage _storage;

    public FilesController(IFileStorage storage)
    {
        _storage = storage;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(20_000_000)]
    public async Task<IActionResult> Upload([FromForm(Name = "file")] IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required.");

        var (fileId, fileName) = await _storage.SaveAsync(file, ct);
        return Ok(new { fileId, fileName });
    }


    [HttpGet("{fileId:guid}")]
    public async Task<IActionResult> Download(Guid fileId, CancellationToken ct)
    {
        var result = await _storage.GetAsync(fileId, ct);
        if (result == null) return NotFound();

        return File(result.Content, result.ContentType, result.FileName);
    }
}
