namespace ApplicantAdmission.DataAccess.Entities;

public class Applicant
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;

    
    public string Phone { get; set; } = null!;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Citizenship { get; set; }

    public ICollection<ApplicantAdmissionEntity> Admissions { get; set; } = new List<ApplicantAdmissionEntity>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}
