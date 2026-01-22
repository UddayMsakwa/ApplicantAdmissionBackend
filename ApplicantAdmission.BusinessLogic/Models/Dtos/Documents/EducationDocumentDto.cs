using System;

namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Documents;

public class EducationDocumentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid DocumentTypeId { get; set; }
    public DateTime IssueDate { get; set; }
}
