namespace ApplicantAdmission.DataAccess.Entities;

public class EducationLevel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<EducationDocumentType> DocumentTypes { get; set; } = new List<EducationDocumentType>();
    public ICollection<Program> Programs { get; set; } = new List<Program>();
}
