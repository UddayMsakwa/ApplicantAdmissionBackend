using FluentValidation;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;

public class ApplicantAdmissionAssignManagerDtoValidator
    : AbstractValidator<ApplicantAdmissionAssignManagerDto>
{
    public ApplicantAdmissionAssignManagerDtoValidator()
    {
        RuleFor(x => x.ManagerId)
            .NotEmpty()
            .WithMessage("ManagerId is required.");
    }
}
