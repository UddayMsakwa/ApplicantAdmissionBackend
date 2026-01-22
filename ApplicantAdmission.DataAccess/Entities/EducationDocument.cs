using System;

namespace ApplicantAdmission.DataAccess.Entities;

public class EducationDocument : Document
{
    public EducationDocument()
    {
        DocumentKind = "Education";
    }

    public Guid DocumentTypeId { get; set; }

    public string Name { get; set; } = "";
    public DateTime IssueDate { get; set; }
}
