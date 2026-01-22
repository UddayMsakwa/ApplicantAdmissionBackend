using System;

namespace ApplicantAdmission.DataAccess.Entities;

public class PassportDocument : Document
{
    public PassportDocument()
    {
        DocumentKind = "Passport";
    }

    public string Series { get; set; } = null!;
    public string Number { get; set; } = null!;
    public string IssuedBy { get; set; } = null!;
    public DateTime IssueDate { get; set; }
    public string BirthPlace { get; set; } = null!;
}
