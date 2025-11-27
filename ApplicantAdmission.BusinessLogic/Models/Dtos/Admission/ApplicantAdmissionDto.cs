namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;

public class ApplicantAdmissionDto
{
    public Guid Id { get; set; }
    public Guid ApplicantId { get; set; }
    public Guid AdmissionProgramId { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant.ApplicantDto Applicant { get; set; } = null!;
    public ApplicantAdmission.BusinessLogic.Models.Dtos.Manager.ManagerDto? Manager { get; set; }
    public ApplicantAdmission.BusinessLogic.Models.Dtos.Program.ProgramDto Program { get; set; } = null!;
}
