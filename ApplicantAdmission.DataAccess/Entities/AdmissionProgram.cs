namespace ApplicantAdmission.DataAccess.Entities;

public class AdmissionProgram
{
    public Guid Id { get; set; }

    public Guid ProgramId { get; set; }
    public ProgramEntity Program { get; set; } = null!;


    public ICollection<ApplicantAdmissionEntity> ApplicantAdmissions { get; set; } = new List<ApplicantAdmissionEntity>();
}
