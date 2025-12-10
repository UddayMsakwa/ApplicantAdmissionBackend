namespace ApplicantAdmission.DataAccess.Entities;

public class ApplicantAdmissionEntity
{
    public Guid Id { get; set; }

    public Guid ApplicantId { get; set; }
    public Applicant Applicant { get; set; } = null!;

    public Guid AdmissionProgramId { get; set; }
    public AdmissionProgram AdmissionProgram { get; set; } = null!;

    public Guid? ManagerId { get; set; }
    public Manager? Manager { get; set; }

    public string Status { get; set; } = "Submitted";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
