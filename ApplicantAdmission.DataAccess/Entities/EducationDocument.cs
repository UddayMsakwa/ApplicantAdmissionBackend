namespace ApplicantAdmission.DataAccess.Entities;

public class EducationDocument : Document
{
    public Guid DocumentTypeId { get; set; }
    public EducationDocumentType DocumentType { get; set; } = null!;

    public string InstitutionName { get; set; } = null!;
    public int GraduationYear { get; set; }
}
