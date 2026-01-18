using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;
using FluentValidation;

namespace ApplicantAdmission.BusinessLogic.Validators;

public class ApplicantUpdateDtoValidator : AbstractValidator<ApplicantUpdateDto>
{
    public ApplicantUpdateDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Phone).NotEmpty();
        RuleFor(x => x.Citizenship).NotEmpty();
        RuleFor(x => x.Gender).NotEmpty();
        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow)
            .WithMessage("Date of birth must be in the past.");
    }
}
