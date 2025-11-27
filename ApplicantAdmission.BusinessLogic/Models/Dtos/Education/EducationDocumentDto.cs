namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Education;

public class EducationDocumentDto
{
    public Guid Id { get; set; }
    public Guid DocumentTypeId { get; set; }
    public string InstitutionName { get; set; } = null!;
    public int GraduationYear { get; set; }
}
