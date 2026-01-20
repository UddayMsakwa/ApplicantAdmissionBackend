using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;
using FluentValidation;

namespace ApplicantAdmission.BusinessLogic.Validators;

public class ApplicantUpdateDtoValidator : AbstractValidator<ApplicantUpdateDto>
{
    public ApplicantUpdateDtoValidator()
    {
        When(x => x.Email != null, () =>
            RuleFor(x => x.Email!)
                .NotEmpty()
                .EmailAddress());

        When(x => x.FullName != null, () =>
            RuleFor(x => x.FullName!)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(200));

        When(x => x.Phone != null, () =>
            RuleFor(x => x.Phone!)
                .NotEmpty()
                .MaximumLength(30));

        When(x => x.Citizenship != null, () =>
            RuleFor(x => x.Citizenship!)
                .NotEmpty()
                .MaximumLength(100));

        When(x => x.DateOfBirth.HasValue, () =>
            RuleFor(x => x.DateOfBirth!.Value)
                .LessThan(DateTime.UtcNow)
                .WithMessage("Date of birth must be in the past."));
    }
}
