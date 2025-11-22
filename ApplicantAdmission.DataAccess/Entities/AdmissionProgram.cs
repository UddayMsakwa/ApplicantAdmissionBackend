namespace ApplicantAdmission.DataAccess.Entities;

public class AdmissionProgram
{
    public Guid Id { get; set; }

    public Guid ProgramId { get; set; }
    public Program Program { get; set; } = null!;

    public ICollection<ApplicantAdmission> ApplicantAdmissions { get; set; } = new List<ApplicantAdmission>();
}
