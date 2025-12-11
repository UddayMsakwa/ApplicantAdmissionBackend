namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Education;

public class EducationDocumentCreateDto
{
    public Guid ApplicantId { get; set; }
    public Guid FileId { get; set; }
    public Guid DocumentTypeId { get; set; }

    public string InstitutionName { get; set; } = null!;
    public int GraduationYear { get; set; }

    public decimal AverageScore { get; set; }
}
