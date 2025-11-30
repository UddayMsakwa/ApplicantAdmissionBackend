namespace ApplicantAdmission.DataAccess.Entities;

public class ProgramEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public Guid FacultyId { get; set; }
    public Faculty Faculty { get; set; } = null!;

    public Guid LevelId { get; set; }
    public EducationLevel Level { get; set; } = null!;

    public ICollection<AdmissionProgram> AdmissionPrograms { get; set; } = new List<AdmissionProgram>();
}
