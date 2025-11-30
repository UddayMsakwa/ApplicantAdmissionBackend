namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Document;

public class DocumentDto
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public string DocumentKind { get; set; } = null!;
}
