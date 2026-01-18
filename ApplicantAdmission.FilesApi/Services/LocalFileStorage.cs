using Microsoft.AspNetCore.StaticFiles;

namespace ApplicantAdmission.FilesApi.Services;

public class LocalFileStorage : IFileStorage
{
    private readonly string _root;

    public LocalFileStorage(IWebHostEnvironment env)
    {
        _root = Path.Combine(env.ContentRootPath, "Uploads");
        Directory.CreateDirectory(_root);
    }

    public async Task<(Guid FileId, string FileName)> SaveAsync(IFormFile file, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        var safeName = Path.GetFileName(file.FileName);
        var path = Path.Combine(_root, $"{id}_{safeName}");

        await using var stream = File.Create(path);
        await file.CopyToAsync(stream, ct);

        return (id, safeName);
    }

    public async Task<FileResultData?> GetAsync(Guid fileId, CancellationToken ct)
    {
        var file = Directory.GetFiles(_root, $"{fileId}_*").FirstOrDefault();
        if (file == null) return null;

        var content = await File.ReadAllBytesAsync(file, ct);
        var name = Path.GetFileName(file).Substring(37);

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(name, out var contentType))
            contentType = "application/octet-stream";

        return new FileResultData
        {
            Content = content,
            ContentType = contentType,
            FileName = name
        };
    }
}
