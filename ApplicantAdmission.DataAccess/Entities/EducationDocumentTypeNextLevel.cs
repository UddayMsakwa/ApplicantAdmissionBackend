namespace ApplicantAdmission.DataAccess.Entities;

public class EducationDocumentTypeNextLevel
{
    public Guid DocumentTypeId { get; set; }
    public EducationDocumentType DocumentType { get; set; } = null!;

    public int NextLevelId { get; set; }
    public EducationLevel NextLevel { get; set; } = null!;
}
