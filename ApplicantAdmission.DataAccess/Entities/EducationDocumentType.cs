namespace ApplicantAdmission.DataAccess.Entities;

public class EducationDocumentType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid LevelId { get; set; }
    public EducationLevel Level { get; set; } = null!;

    public Guid? NextLevelId { get; set; }
    public EducationLevel? NextLevel { get; set; }
}
