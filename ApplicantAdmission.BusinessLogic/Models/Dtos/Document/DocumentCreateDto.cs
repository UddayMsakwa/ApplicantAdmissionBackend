namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Document;

public class DocumentCreateDto
{
    public Guid ApplicantId { get; set; }
    public Guid FileId { get; set; }
    public string DocumentKind { get; set; } = null!;
}
