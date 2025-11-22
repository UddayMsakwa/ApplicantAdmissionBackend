namespace ApplicantAdmission.DataAccess.Entities;

public class PassportDocument : Document
{
    public string Number { get; set; } = null!;
    public string Country { get; set; } = null!;
    public DateTime IssueDate { get; set; }
}
