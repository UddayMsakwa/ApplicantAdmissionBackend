namespace ApplicantAdmission.DataAccess.Entities;

public class AdmissionProgram
{
    public Guid Id { get; set; }

    public Guid ApplicantAdmissionId { get; set; }
    public ApplicantAdmissionEntity ApplicantAdmission { get; set; } = null!;

    public Guid ProgramId { get; set; }       
    public ProgramEntity Program { get; set; } = null!;

    public int Priority { get; set; }
}

