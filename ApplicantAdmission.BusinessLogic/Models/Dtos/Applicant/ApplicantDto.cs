namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;

public class ApplicantDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
}
