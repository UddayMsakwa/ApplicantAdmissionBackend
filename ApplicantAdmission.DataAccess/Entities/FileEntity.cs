namespace ApplicantAdmission.DataAccess.Entities;

public class FileEntity
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long Size { get; set; }

    public ICollection<Document> Documents { get; set; } = new List<Document>();
}
