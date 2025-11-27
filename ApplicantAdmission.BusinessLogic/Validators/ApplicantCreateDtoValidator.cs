using FluentValidation;
using ApplicantAdmission.BusinessLogic.Models;

public class ApplicantCreateDtoValidator : AbstractValidator<ApplicantCreateDto>
{
    public ApplicantCreateDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Phone)
            .NotEmpty();

        RuleFor(x => x.Citizenship)
            .NotEmpty();

        RuleFor(x => x.Gender)
            .NotEmpty();

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow)
            .WithMessage("Date of birth must be in the past.");
    }
}
