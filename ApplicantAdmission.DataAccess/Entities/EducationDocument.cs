using System;

namespace ApplicantAdmission.DataAccess.Entities;

public class EducationDocument : Document
{
    public EducationDocument()
    {
        DocumentKind = "Education";
    }

    public Guid DocumentTypeId { get; set; }
    public string InstitutionName { get; set; } = null!;
    public int GraduationYear { get; set; }
    public decimal AverageScore { get; set; }
}


