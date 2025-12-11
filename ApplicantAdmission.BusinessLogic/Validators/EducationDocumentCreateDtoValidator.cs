using FluentValidation;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Education;

public class EducationDocumentCreateDtoValidator
    : AbstractValidator<EducationDocumentCreateDto>
{
    public EducationDocumentCreateDtoValidator()
    {
        RuleFor(x => x.ApplicantId).NotEmpty();
        RuleFor(x => x.FileId).NotEmpty();
        RuleFor(x => x.DocumentTypeId).NotEmpty();

        RuleFor(x => x.InstitutionName)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.GraduationYear)
            .InclusiveBetween(1950, DateTime.UtcNow.Year);

        RuleFor(x => x.AverageScore)
            .InclusiveBetween(0, 100)
            .WithMessage("AverageScore must be between 0 and 100.");
    }
}
