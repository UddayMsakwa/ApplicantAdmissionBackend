using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace ApplicantAdmission.BusinessLogic.Services;

public sealed class FilesApiClient
{
    private readonly HttpClient _http;

    public FilesApiClient(HttpClient http) => _http = http;

    public async Task<(Guid FileId, string FileName)> UploadAsync(IFormFile file, string bearerToken, CancellationToken ct)
    {
        using var form = new MultipartFormDataContent();

       
        await using var stream = file.OpenReadStream();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");

        form.Add(fileContent, "file", file.FileName);

        using var req = new HttpRequestMessage(HttpMethod.Post, "files/upload")
        {
            Content = form
        };
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        using var res = await _http.SendAsync(req, ct);
        var body = await res.Content.ReadAsStringAsync(ct);

        if (!res.IsSuccessStatusCode)
            throw new HttpRequestException($"FilesApi upload failed: {(int)res.StatusCode} {res.ReasonPhrase}. Body: {body}");

        
        var json = System.Text.Json.JsonDocument.Parse(body).RootElement;
        var fileId = json.GetProperty("fileId").GetGuid();
        var fileName = json.GetProperty("fileName").GetString() ?? file.FileName;

        return (fileId, fileName);
    }

    public async Task<(byte[] Content, string ContentType, string FileName)?> DownloadAsync(Guid fileId, string bearerToken, CancellationToken ct)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, $"files/{fileId}");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        using var res = await _http.SendAsync(req, ct);
        if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

        var bytes = await res.Content.ReadAsByteArrayAsync(ct);
        var contentType = res.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

        var fileName = res.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? $"{fileId}";
        return (bytes, contentType, fileName);
    }
}
