namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;

public class ApplicantDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = null!;
    public string Citizenship { get; set; } = null!;
}
