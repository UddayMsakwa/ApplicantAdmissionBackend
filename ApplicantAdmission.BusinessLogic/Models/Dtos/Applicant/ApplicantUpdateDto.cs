namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;

public class ApplicantUpdateDto
{
    
    public string? FullName { get; set; }
    public string? Phone { get; set; }

    
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Citizenship { get; set; }
    public string? Email { get; set; }
}
