using FluentValidation;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Manager;

public class ManagerDtoValidator : AbstractValidator<ManagerDto>
{
    public ManagerDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Role).NotEmpty();
    }
}
