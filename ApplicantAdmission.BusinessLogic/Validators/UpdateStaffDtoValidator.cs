using ApplicantAdmission.BusinessLogic.Models.Dtos.Admin;
using ApplicantAdmission.DataAccess.Enums;
using FluentValidation;

namespace ApplicantAdmission.BusinessLogic.Validators;

public class UpdateStaffDtoValidator : AbstractValidator<UpdateStaffDto>
{
    public UpdateStaffDtoValidator()
    {
        When(x => x.Email != null, () =>
        {
            RuleFor(x => x.Email!).EmailAddress();
        });

        When(x => x.Password != null, () =>
        {
            RuleFor(x => x.Password!).MinimumLength(6);
        });

        When(x => x.Role.HasValue, () =>
        {
            RuleFor(x => x.Role!.Value)
                .Must(r => r != UserRole.Applicant)
                .WithMessage("Cannot set role to Applicant here.");
        });
    }
}
