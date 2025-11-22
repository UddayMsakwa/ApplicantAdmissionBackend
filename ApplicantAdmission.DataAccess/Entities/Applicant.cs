namespace ApplicantAdmission.DataAccess.Entities;

public class Applicant
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = null!;
    public string Citizenship { get; set; } = null!;

    public ICollection<ApplicantAdmission> Admissions { get; set; } = new List<ApplicantAdmission>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}
