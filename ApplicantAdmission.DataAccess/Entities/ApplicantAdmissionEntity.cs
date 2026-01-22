
using ApplicantAdmission.DataAccess.Enums;

namespace ApplicantAdmission.DataAccess.Entities;

public class ApplicantAdmissionEntity
{
    public Guid Id { get; set; }

    public Guid ApplicantId { get; set; }
    public Applicant Applicant { get; set; } = null!;

    public Guid? ManagerUserId { get; set; }
    public UserEntity? ManagerUser { get; set; }

    public AdmissionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    
    public DateTime LastModifiedAt { get; set; }

    public ICollection<AdmissionProgram> AdmissionPrograms { get; set; } = new List<AdmissionProgram>();
}
