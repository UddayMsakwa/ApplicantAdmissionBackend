using System;

namespace ApplicantAdmission.DataAccess.Entities;

public class DocumentScan
{
    public Guid Id { get; set; }

    public Guid DocumentId { get; set; }
    public Document Document { get; set; } = null!;

    public Guid FileId { get; set; }

    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long Size { get; set; }

    public DateTime CreatedAt { get; set; }
}
