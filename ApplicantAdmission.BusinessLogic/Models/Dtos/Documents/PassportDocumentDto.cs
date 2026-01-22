using System;

namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Documents;

public class PassportDocumentDto
{
    public Guid Id { get; set; }
    public string Series { get; set; } = null!;
    public string Number { get; set; } = null!;
    public string IssuedBy { get; set; } = null!;
    public DateTime IssueDate { get; set; }
    public string BirthPlace { get; set; } = null!;
}
