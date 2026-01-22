using ApplicantAdmission.DataAccess.Enums;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Manager;
using ApplicantAdmission.BusinessLogic.Models.Dtos.SelectedPrograms;

namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;

public class ApplicantAdmissionDto
{
    public Guid Id { get; set; }
    public Guid ApplicantId { get; set; }
    public AdmissionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public ApplicantDto Applicant { get; set; } = null!;
    public ManagerDto? Manager { get; set; }

    
    public List<SelectedProgramDto> SelectedPrograms { get; set; } = new();
}
