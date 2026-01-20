namespace ApplicantAdmission.DataAccess.Entities;

public class EducationDocumentType
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";

    
    public int LevelId { get; set; }
    public EducationLevel Level { get; set; } = null!;

    
    public ICollection<EducationDocumentTypeNextLevel> NextLevels { get; set; }
        = new List<EducationDocumentTypeNextLevel>();
}
