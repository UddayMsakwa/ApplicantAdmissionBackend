using System;

namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Documents;

public class DocumentScanDto
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; }
}
