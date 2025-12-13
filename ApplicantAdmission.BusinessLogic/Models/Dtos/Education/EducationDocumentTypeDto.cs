namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Education;

public class EducationDocumentTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid LevelId { get; set; }
    public Guid? NextLevelId { get; set; }
}
