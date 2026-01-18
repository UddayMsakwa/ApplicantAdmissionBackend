namespace ApplicantAdmission.FilesApi.Services;

public interface IFileStorage
{
    Task<(Guid FileId, string FileName)> SaveAsync(IFormFile file, CancellationToken ct);
    Task<FileResultData?> GetAsync(Guid fileId, CancellationToken ct);
}

public sealed class FileResultData
{
    public required byte[] Content { get; init; }
    public required string ContentType { get; init; }
    public required string FileName { get; init; }
}
