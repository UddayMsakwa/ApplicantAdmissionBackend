using FluentValidation;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Education;

public class EducationDocumentCreateDtoValidator
    : AbstractValidator<EducationDocumentCreateDto>
{
    public EducationDocumentCreateDtoValidator()
    {
        RuleFor(x => x.ApplicantId)
            .NotEmpty()
            .WithMessage("ApplicantId is required.");

        RuleFor(x => x.FileId)
            .NotEmpty()
            .WithMessage("FileId is required.");

        RuleFor(x => x.DocumentTypeId)
            .NotEmpty()
            .WithMessage("DocumentTypeId is required.");

        RuleFor(x => x.InstitutionName)
            .NotEmpty()
            .WithMessage("InstitutionName is required.")
            .MinimumLength(3)
            .WithMessage("InstitutionName must be at least 3 characters long.");

        RuleFor(x => x.GraduationYear)
            .InclusiveBetween(1950, DateTime.UtcNow.Year)
            .WithMessage("GraduationYear must be a valid past year.");

        RuleFor(x => x.AverageScore)
            .InclusiveBetween(0, 100)
            .WithMessage("AverageScore must be between 0 and 100.");
    }
}
