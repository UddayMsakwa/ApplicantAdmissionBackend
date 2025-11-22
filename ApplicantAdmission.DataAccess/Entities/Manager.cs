namespace ApplicantAdmission.DataAccess.Entities;

public class Manager
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FullName { get; set; } = null!;

    public string Role { get; set; } = null!; // Manager, HeadManager, Admin

    public ICollection<ApplicantAdmission> Admissions { get; set; } = new List<ApplicantAdmission>();
}
