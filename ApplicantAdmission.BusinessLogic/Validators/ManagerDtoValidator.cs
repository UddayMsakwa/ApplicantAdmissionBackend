using ApplicantAdmission.BusinessLogic.Models.Dtos.Manager;
using FluentValidation;

namespace ApplicantAdmission.BusinessLogic.Validators;

public class ManagerDtoValidator : AbstractValidator<ManagerDto>
{
    public ManagerDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Role).NotEmpty();
    }
}
