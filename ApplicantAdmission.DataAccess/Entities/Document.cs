using System;
namespace ApplicantAdmission.DataAccess.Entities;

public abstract class Document
{
    public Guid Id { get; set; }

    public Guid ApplicantId { get; set; }
    public Applicant Applicant { get; set; } = null!;

    public Guid FileId { get; set; }
    public FileEntity File { get; set; } = null!;

    public string DocumentKind { get; set; } = null!; // "Passport", "Education"
}
