using ApplicantAdmission.BusinessLogic.Models.Dtos.Admin;
using ApplicantAdmission.DataAccess.Enums;
using FluentValidation;

namespace ApplicantAdmission.BusinessLogic.Validators;

public class CreateStaffDtoValidator : AbstractValidator<CreateStaffDto>
{
    public CreateStaffDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.TempPassword).NotEmpty().MinimumLength(6);

        RuleFor(x => x.Role)
            .IsInEnum()
            .Must(r => r != UserRole.Applicant)
            .WithMessage("Role must be Admin, HeadManager, or Manager.");

        When(x => x.Role == UserRole.Manager, () =>
        {
            RuleFor(x => x.FullName).NotEmpty().MinimumLength(3);
        });
    }
}
