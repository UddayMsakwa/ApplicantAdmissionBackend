using FluentValidation;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Education;

public class EducationDocumentDtoValidator : AbstractValidator<EducationDocumentDto>
{
    public EducationDocumentDtoValidator()
    {
        RuleFor(x => x.DocumentTypeId).NotEmpty();
        RuleFor(x => x.InstitutionName).NotEmpty();
        RuleFor(x => x.GraduationYear)
            .InclusiveBetween(1950, DateTime.UtcNow.Year);
    }
}
