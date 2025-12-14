using FluentValidation;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;

public class ApplicantCreateDtoValidator
    : AbstractValidator<ApplicantCreateDto>
{
    public ApplicantCreateDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("FullName is required.")
            .MinimumLength(3)
            .WithMessage("FullName must be at least 3 characters long.");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Phone is required.");

        RuleFor(x => x.Citizenship)
            .NotEmpty()
            .WithMessage("Citizenship is required.");

        RuleFor(x => x.Gender)
            .NotEmpty()
            .WithMessage("Gender is required.");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow)
            .WithMessage("DateOfBirth must be in the past.");
    }
}
